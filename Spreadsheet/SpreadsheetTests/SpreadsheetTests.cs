using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
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
        /// Test that SetCellContents throws an exception when name is null on double.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 1);
        }

        /// <summary>
        /// Test that SetCellContents throws an exception when name is null on text.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, "1");
        }

        /// <summary>
        /// Test that SetCellContents throws an exception when text is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMethod4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", null);
        }

        /// <summary>
        /// Test that SetCellContents throws an exception when name is null on formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, new Formula());
        }

        /// <summary>
        /// Test a simple set and get contents with double.
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", 3);
            Assert.AreEqual((double) s.GetCellContents("A1"), 3);
        }

        /// <summary>
        /// Test a second set and get contents with a Formula.
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", new Formula("3 + 1"));
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
            s.SetCellContents("a1", "testing");
            Assert.AreEqual(s.GetCellContents("A1").ToString(), "testing");
        }

        /// <summary>
        /// Test a setcontents returns an ISet of dependent cells for formula.
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("b1", new Formula("a1 * 2"));
            s.SetCellContents("c1", new Formula("b1 + a1"));
            Assert.IsTrue(s.SetCellContents("A1", new Formula()).SetEquals(new List<string> { "A1", "B1", "C1" }));
        }

        /// <summary>
        /// Test a setcontents returns an ISet of dependent cells for double.
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("b1", new Formula("a1 * 2"));
            s.SetCellContents("c1", new Formula("b1 + a1"));
            Assert.IsTrue(s.SetCellContents("A1", 3).SetEquals(new List<string> { "A1", "B1", "C1" }));
        }

        /// <summary>
        /// Test a setcontents returns an ISet of dependent cells for text.
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("b1", new Formula("a1 * 2"));
            s.SetCellContents("c1", new Formula("b1 + a1"));
            Assert.IsTrue(s.SetCellContents("A1", "testing").SetEquals(new List<string> { "A1", "B1", "C1" }));
        }

        /// <summary>
        /// Test GetCellContents for a cell containing a Formula.
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("b1", new Formula("a1 * 2"));
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
            s.SetCellContents("b1", "testing");
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
            s.SetCellContents("b1", 5);
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
            s.SetCellContents("b1", 5);
            s.SetCellContents("a1", new Formula());
            s.SetCellContents("a5", "test");
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
            s.SetCellContents("a1", new Formula("b1"));
            s.SetCellContents("b1", new Formula("a1"));
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
            s.SetCellContents("A05", "testing");
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod19()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("x", "testing");
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod20()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("x", 5);
        }

        /// <summary>
        /// Test an invalid cell name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod21()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("x", new Formula());
        }

        /// <summary>
        /// Test GetNamesOfAllNonemptyCells with a deep Circular dependency.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethod22()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", new Formula("b1"));
            s.SetCellContents("b1", new Formula("c1"));
            s.SetCellContents("c1", new Formula("d1"));
            s.SetCellContents("d1", new Formula("a1"));
        }

    }
}
