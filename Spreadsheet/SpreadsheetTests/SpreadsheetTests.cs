using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;

namespace SpreadsheetTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SpreadsheetTests
    {

        /// <summary>
        /// Test that spreadsheet can be an Abstract Spreadsheet.
        /// </summary>
        [TestMethod]
        public void TestMethod0()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsTrue(s.GetType() == typeof(Spreadsheet));
        }

        /// <summary>
        /// Test that GetCellContents throws an exception when name is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        /// <summary>
        /// Test that SetContentsOfCell throws an exception when name is null on double.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1");
        }

        /// <summary>
        /// Test that SetContentsOfCell throws an exception when name is null on text.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1");
        }

        /// <summary>
        /// Test that SetContentsOfCell throws an exception when text is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMethod4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", null);
        }

        /// <summary>
        /// Test that SetContentsOfCell throws an exception when name is null on formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=");
        }

        /// <summary>
        /// Test a simple set and get contents with double.
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "3");
            Assert.AreEqual((double) s.GetCellContents("A1"), 3);
        }

        /// <summary>
        /// Test a second set and get contents with a Formula.
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=3 + 1");
            string contents = Regex.Replace(s.GetCellContents("A1").ToString(), @"\s+", string.Empty);
            Assert.AreEqual(contents, "3+1");
        }

        /// <summary>
        /// Test a simple set and get contents with text.
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "testing");
            Assert.AreEqual(s.GetCellContents("A1").ToString(), "testing");
        }

        /// <summary>
        /// Test a setcontents returns an ISet of dependent cells for formula.
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "=a1 * 2");
            s.SetContentsOfCell("c1", "=b1 + a1");
            Assert.IsTrue(s.SetContentsOfCell("A1", "=").SetEquals(new List<string> { "A1", "B1", "C1" }));
        }

        /// <summary>
        /// Test a setcontents returns an ISet of dependent cells for double.
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "=a1 * 2");
            s.SetContentsOfCell("c1", "=b1 + a1");
            Assert.IsTrue(s.SetContentsOfCell("A1", "3").SetEquals(new List<string> { "A1", "B1", "C1" }));
        }

        /// <summary>
        /// Test a setcontents returns an ISet of dependent cells for text.
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "=a1 * 2");
            s.SetContentsOfCell("c1", "=b1 + a1");
            Assert.IsTrue(s.SetContentsOfCell("A1", "testing").SetEquals(new List<string> { "A1", "B1", "C1" }));
        }

        /// <summary>
        /// Test GetCellContents for a cell containing a Formula.
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "=a1 * 2");
            Assert.IsTrue(s.GetCellContents("b1") is Formula);
            Assert.IsFalse(s.GetCellContents("b1") is string);
            Assert.IsFalse(s.GetCellContents("b1") is double);
        }

        /// <summary>
        /// Test GetCellContents for a cell containing a string.
        /// </summary>
        [TestMethod]
        public void TestMethod13()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "testing");
            Assert.IsTrue(s.GetCellContents("b1") is string);
            Assert.IsFalse(s.GetCellContents("b1") is Formula);
            Assert.IsFalse(s.GetCellContents("b1") is double);
        }

        /// <summary>
        /// Test GetCellContents for a cell containing a double.
        /// </summary>
        [TestMethod]
        public void TestMethod14()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "5");
            Assert.IsTrue(s.GetCellContents("b1") is double);
            Assert.IsFalse(s.GetCellContents("b1") is string);
            Assert.IsFalse(s.GetCellContents("b1") is Formula);
        }

        /// <summary>
        /// Test GetNamesOfAllNonemptyCells.
        /// </summary>
        [TestMethod]
        public void TestMethod15()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "5");
            s.SetContentsOfCell("a1", "=");
            s.SetContentsOfCell("a5", "test");
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Count() == 3);
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("b1", StringComparer.CurrentCultureIgnoreCase));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("a5", StringComparer.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Test GetNamesOfAllNonemptyCells.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethod16()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "=a1");
        }

        /// <summary>
        /// Test GetNamesOfAllNonemptyCells has nothing.
        /// </summary>
        [TestMethod]
        public void TestMethod17()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod18()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A05", "testing");
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod19()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("x", "testing");
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod20()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("x", "5");
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod21()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("x", "=");
        }

        /// <summary>
        /// Test GetNamesOfAllNonemptyCells with a deep Circular dependency.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethod22()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "=c1");
            s.SetContentsOfCell("c1", "=d1");
            s.SetContentsOfCell("d1", "=a1");
        }

        /// <summary>
        /// Test that a spreadsheet has changed when setting a cell.
        /// </summary>
        [TestMethod]
        public void TestMethod23()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.Changed);
            s.SetContentsOfCell("a1", "=b1");
            Assert.IsTrue(s.Changed);
        }

        /// <summary>
        /// Test that a spreadsheet has changed when setting a cell.
        /// </summary>
        [TestMethod]
        public void TestMethod24()
        {
            Spreadsheet s = new Spreadsheet(new Regex(".*"));
            Assert.IsFalse(s.Changed);
            s.SetContentsOfCell("a1", "=b1");
            Assert.IsTrue(s.Changed);
        }

        static readonly string TestXMLPath = Environment.CurrentDirectory + "/testXML.xml";
        static readonly string TestXMLPath2 = Environment.CurrentDirectory + "/testXML1.xml";

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Assert.Fail("XML Validation Warning: " + args.Message);
            else if (args.Severity == XmlSeverityType.Error)
                Assert.Fail("XML Validation Error: " + args.Message);

            Console.WriteLine(args.Message);
        }

        private XmlDocument LoadTestDocument(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);

            //Add the schema to the document.
            XmlTextReader reader = new XmlTextReader("Spreadsheet.xsd");
            XmlSchemaSet spreadsheetSchema = new XmlSchemaSet();
            spreadsheetSchema.Add(null, reader);

            document.Schemas.Add(spreadsheetSchema);

            return document;
        }

        /// <summary>
        /// Test Saving a xml doc and that it validates against the schema.
        /// </summary>
        [TestMethod]
        public void TestXML1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "hello");
            s.SetContentsOfCell("b1", "=a2");
            s.SetContentsOfCell("c1", "5");

            //Create the xml.
            TextWriter writer = File.CreateText(TestXMLPath);
            s.Save(writer);
            writer.Close(); // Close the writer to avoid open collision errors.

            //Assert that the document exists.
            Assert.IsTrue(File.Exists(TestXMLPath));

            //Assert that the document is valid with the schema.
            XmlDocument document = LoadTestDocument(TestXMLPath);
            document.Validate(ValidationCallback);
        }

        /// <summary>
        /// Test Saving a xml doc and that it opens and reads back in.
        /// </summary>
        [TestMethod]
        public void TestXML2()
        {
            TestXML1();
            TextReader reader = File.OpenText(TestXMLPath);
            Spreadsheet s = new Spreadsheet(reader);
            Assert.AreEqual("hello", s.GetCellContents("a1"));
            Assert.AreEqual("A2", s.GetCellContents("b1").ToString());
            Assert.AreEqual(5.0, s.GetCellContents("c1"));
        }

        /// <summary>
        /// Test Saving a xml doc with a formula and double and that it validates against the schema.
        /// </summary>
        [TestMethod]
        public void TestXML3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b1", "=c1+d1");
            s.SetContentsOfCell("c1", "5");

            //Create the xml.
            TextWriter writer = File.CreateText(TestXMLPath2);
            s.Save(writer);
            writer.Close(); // Close the writer to avoid open collision errors.

            //Assert that the document exists.
            Assert.IsTrue(File.Exists(TestXMLPath2));

            //Assert that the document is valid with the schema.
            XmlDocument document = LoadTestDocument(TestXMLPath2);
            document.Validate(ValidationCallback);

            //Assert that cells are written correctly.
            foreach (XmlElement cell in document.GetElementsByTagName("cell"))
            {
                string cellName = cell.GetAttribute("name");
                switch (cellName)
                {
                    case "B1":
                        Assert.AreEqual(Regex.Replace(cell.GetAttribute("contents"), @"\s", ""), "=C1+D1");
                        break;
                    case "C1":
                        Assert.AreEqual(cell.GetAttribute("contents"), "5");
                        break;
                }
            }
        }

        /// <summary>
        /// Test get value of a cell with a formula.
        /// </summary>
        [TestMethod]
        public void TestSpreadsheetValues1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=1+1");
            Assert.AreEqual(s.GetCellValue("a1"), 2.0);
        }

        /// <summary>
        /// Test get value of an empty cell.
        /// </summary>
        [TestMethod]
        public void TestSpreadsheetValues2()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(s.GetCellValue("a1"), string.Empty);
        }

        /// <summary>
        /// Test get value of a cell with a double.
        /// </summary>
        [TestMethod]
        public void TestSpreadsheetValues3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            Assert.AreEqual(s.GetCellValue("a1"), 5.0);
        }

        /// <summary>
        /// Test get value of a cell with a string.
        /// </summary>
        [TestMethod]
        public void TestSpreadsheetValues4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "cool!");
            Assert.AreEqual(s.GetCellValue("a1"), "cool!");
        }

        /// <summary>
        /// Stress Test the get values.
        /// </summary>
        [TestMethod]
        public void StressTestSpreadsheetValues()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "1");
            for (int i = 2; i <= 250; i++)
            {
                s.SetContentsOfCell($"a{i}", $"=a{i - 1} + 1");
            }
            Assert.AreEqual(250.0, (double)s.GetCellValue("a250"), 1e-9);
            Assert.AreEqual(100.0, (double)s.GetCellValue("a100"), 1e-9);
        }
    }
}
