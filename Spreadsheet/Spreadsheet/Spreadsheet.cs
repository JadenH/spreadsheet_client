using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            return _cells[name].GetCellContents();
        }

        /// <summary>
        /// Validates that a given cellName contains a letter followed by a number.
        /// </summary>
        private void ValidateCellName(string name)
        {
            Regex regex = new Regex($"[a-zA-Z]+([1-9][0-9]*)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            if (!regex.IsMatch(name)) throw new InvalidNameException();
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
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            Cell cell = new Cell(number);
            _cells.Add(name, cell);

            HashSet<string> dependents = new HashSet<string>(_dependencyGraph.GetDependees(name).ToList()) { name };
            return dependents;
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
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            Cell cell = new Cell(text);
            _cells.Add(name, cell);

            HashSet<string> dependents = new HashSet<string>(_dependencyGraph.GetDependees(name).ToList()) { name };
            return dependents;
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
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null) throw new InvalidNameException();
            name = name.ToUpper();
            ValidateCellName(name);

            foreach (var token in formula.GetVariables())
            {
                _dependencyGraph.AddDependency(name, token.ToUpper());
            }

            try
            {
                GetCellsToRecalculate(name);
            }
            catch (CircularException)
            {
                foreach (var token in formula.GetVariables())
                {
                    _dependencyGraph.RemoveDependency(name, token);
                }
                throw new CircularException();
            }

            Cell cell = new Cell(formula);
            _cells.Add(name, cell);


            HashSet<string> dependents = new HashSet<string>(_dependencyGraph.GetDependees(name).ToList()) {name};
            return dependents;
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
            return _dependencyGraph.GetDependents(name);
        }
    }
}
