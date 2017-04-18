using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Dependencies;
using Formulas;

namespace SS 
{
    /// <summary>
    /// Inherits from Abstract Spreadsheet. Contains cells and dependencies of cells.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private readonly DependencyGraph _dependencyGraph = new DependencyGraph();
        private readonly Dictionary<string, Cell> _cells = new Dictionary<string, Cell>();
        private bool _changed;
        private const string _validCellNamePattern = "^[a-zA-Z]+([1-9][0-9]*)$";
        private Regex _isValid;

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet()
        {
            _isValid = new Regex(".*");
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter.
        /// </summary>
        public Spreadsheet(Regex isValid)
        {
            _isValid = isValid;
        }

        /// <summary>
        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  If there's a problem reading source, throws an IOException
        /// If the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  If there is an invalid cell name, or a 
        /// duplicate cell name, or an invalid formula in the source, throws a SpreadsheetReadException.
        /// If there's a Formula that causes a circular dependency, throws a SpreadsheetReadException. 
        /// </summary>
        public Spreadsheet(TextReader source)
        {
            //Create the XML Document.
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(source);
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }

            //Add the schema to the document.
            XmlTextReader reader = new XmlTextReader("Spreadsheet.xsd");
            XmlSchemaSet spreadsheetSchema = new XmlSchemaSet();
            spreadsheetSchema.Add(null, reader);
            document.Schemas.Add(spreadsheetSchema);

            //Validate document.
            document.Validate((sender, args) =>
            {
                if (args.Severity == XmlSeverityType.Error)
                {
                    throw new SpreadsheetReadException("Error opening file. Invalid schema.");
                }
            });

            XmlElement spreadsheetElement = document.GetElementsByTagName("spreadsheet")[0] as XmlElement;
            _isValid = new Regex(spreadsheetElement.GetAttribute("IsValid"));

            XmlNodeList cells = document.GetElementsByTagName("cell");
            HashSet<string> addedCells = new HashSet<string>();

            //Iterate through cells in the document.
            foreach (XmlElement cell in cells)
            {
                //Verify cell name is unique.
                string cellName = cell.GetAttribute("name");
                if (addedCells.Contains(cellName)) throw new SpreadsheetReadException("Spreadsheet contains duplicate values for cell name: " + cellName);
                addedCells.Add(cell.GetAttribute("name"));

                //Add the cell to the spreadsheet.
                try
                {
                    SetContentsOfCell(cellName, cell.GetAttribute("contents"));
                }
                catch (CircularException e)
                {
                    throw new SpreadsheetReadException("Error in spreadsheet: " + e.Message);
                }
                catch (FormulaFormatException e)
                {
                    throw new SpreadsheetReadException("Error in spreadsheet: " + e.Message);
                }
            }
            Changed = false;
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return _changed;
            }
            protected set { _changed = value; }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the isvalid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            XmlDocument document = new XmlDocument();
            //Setup xml document with UTF-8 encoding and specified Schema.
            XmlTextReader reader = new XmlTextReader("Spreadsheet.xsd");

            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = document.DocumentElement;
            document.InsertBefore(xmlDeclaration, root);

            XmlSchema myschema = XmlSchema.Read(reader, ValidationCallback);
            document.Schemas.Add(myschema);

            //Create the spreadsheet element.
            XmlElement spreadsheet = document.CreateElement("spreadsheet");
            spreadsheet.SetAttribute("IsValid", _isValid.ToString());

            //Create the cell elements.
            foreach (var cell in _cells)
            {
                XmlElement element = document.CreateElement("cell");
                element.SetAttribute("name", cell.Key);
                string val = GetCellContents(cell.Key).ToString();
                if (GetCellContents(cell.Key) is Formula) val = "=" + val;
                element.SetAttribute("contents", val);
                spreadsheet.AppendChild(element);
            }

            document.AppendChild(spreadsheet);

            //Save the xml document.
            Changed = false;
            document.Validate(ValidationCallback);

            document.Save(dest);
        }

        /// <summary>
        /// Used as the callback for validating xml against the schema.
        /// </summary>
        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            Debug.Assert(args.Severity != XmlSeverityType.Warning);
            Debug.Assert(args.Severity != XmlSeverityType.Error);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            return _cells.ContainsKey(name) ? _cells[name].GetValue(_cells) : string.Empty;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return _cells.Select(cell => cell.Key);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            return _cells.ContainsKey(name) ? _cells[name].GetCellContents() : string.Empty;
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            Type contentType = GetContentType(content);
            Changed = true;

            if (contentType == typeof(double)) return SetCellContents(name, double.Parse(content));
            if (contentType == typeof(Formula))
            {
                //Should always be successful since we check that it's a formula prior to this.
                Match match = new Regex("=(.*)").Match(content);

                //If our formula has a token.
                if (match.Groups[1].Value.Length > 0)
                {
                    return SetCellContents(name,
                        new Formula(match.Groups[1].Value, s => s.ToUpper(),
                            s => new Regex(_validCellNamePattern).IsMatch(s)));
                }
                //If our formula has no tokens.
                return SetCellContents(name, new Formula());
            }
            return SetCellContents(name, content);
        }

        /// <summary>
        /// Returns the content type given a string. Can return a string, double or Formula.
        /// </summary>
        private Type GetContentType(string content)
        {
            if (Regex.IsMatch(content, @"^=.*$")) return typeof(Formula);
            if (Regex.IsMatch(content, @"^(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?$",
                RegexOptions.IgnorePatternWhitespace)) return typeof(double);
            return typeof(string);
        }

        /// <summary>
        /// Validates that a given cellName contains a letter followed by a number.
        /// </summary>
        private void ValidateCellName(string name)
        {
            if (!new Regex(_validCellNamePattern).IsMatch(name)) throw new InvalidNameException();
            if (!_isValid.IsMatch(name.ToUpper())) throw new InvalidNameException();
        }

        /// <summary>
        /// Recalculates the given cell and all the cells that are dependent on the cell.
        /// </summary>
        private void RecalculateCells(string name)
        {
            foreach (var cell in GetCellsToRecalculate(name))
            {
                if (_cells.ContainsKey(cell))
                {
                    _cells[cell].Recalculate(_cells);
                }
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);
            if (_cells.ContainsKey(name)) _cells.Remove(name);

            Cell cell = new Cell(number);
            _cells.Add(name, cell);

            RecalculateCells(name);
            return new HashSet<string>(GetCellsToRecalculate(name)) { name };
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (name == null) throw new InvalidNameException();
            if (_cells.ContainsKey(name)) _cells.Remove(name);
            if (text != string.Empty)
            {
                name = name.ToUpper();
                ValidateCellName(name);

                Cell cell = new Cell(text);
                _cells.Add(name, cell);
                RecalculateCells(name);
            }
            

            return new HashSet<string>(GetCellsToRecalculate(name)) { name };
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            foreach (var token in formula.GetVariables())
            {
                try
                {
                    ValidateCellName(token.ToUpper());
                }
                catch (Exception e)
                {
                    throw new FormulaFormatException(e.Message);
                }
                _dependencyGraph.AddDependency(token.ToUpper(), name);
            }

            bool isCycle = false;

            try
            {
                GetCellsToRecalculate(name);
            }
            catch (CircularException)
            {
                isCycle = true;
            }

            if (_cells.ContainsKey(name)) _cells.Remove(name);

            Cell cell = new Cell(formula);
            _cells.Add(name, cell);

            if(isCycle)
                MarkCycle(name);

            RecalculateCells(name);
            return new HashSet<string>(GetCellsToRecalculate(name)) { name };
        }

        private void MarkCycle(string cell)
        {
            Console.WriteLine("Marking " + cell);
            if (!_cells.ContainsKey(cell))
            {
                Console.WriteLine("okay so this does happen!");
            }

            if (_cells[cell].IsCircular == true)
                return;

            Console.WriteLine("Setting circular");
            _cells[cell].IsCircular = true;
            _cells[cell].Recalculate(_cells);

            foreach (string s in _dependencyGraph.GetDependents(cell))
            {
                MarkCycle(s);
            }
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null) throw new ArgumentNullException();
            ValidateCellName(name);
            foreach (var dependent in _dependencyGraph.GetDependents(name))
            {
                yield return dependent;
            }
        }
    }
}
