// Written by Joe Zachary for CS 3500, January 2016.
// Repaired error in Evaluate5.  Added TestMethod Attribute
//    for Evaluate4 and Evaluate5 - JLZ January 25, 2016
// Corrected comment for Evaluate3 - JLZ January 29, 2016

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UnitTests
    {
        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// This tests that there must be at least one token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct4()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// This tests reading tokens from left to right, at no point should the number of closing 
        /// parentheses seen so far be greater than the number of opening parentheses seen so far.
        /// Should throw a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula(")(2 + 2) * 2");
        }

        /// <summary>
        /// Tests that a ForumlaFormatException is thrown when
        /// there are unequal amount of closing and opening parentheses.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("((2 + 2) * 2");
        }

        /// <summary>
        /// Tests that a negative floating point number causes an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7()
        {
            Formula f = new Formula("-5.3");
        }

        /// <summary>
        /// Tests a invalid last token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct8()
        {
            Formula f = new Formula("2.5e9 + x5 /");
        }

        /// <summary>
        /// Tests a few valid formulas to make sure there are no exceptions.
        /// </summary>
        [TestMethod]
        public void Construct9()
        {
            Formula f = new Formula("2.5e9 + x5 / 17");
            Formula f1 = new Formula("x*y-2+35/9");
            Formula f2 = new Formula("(5 * 2) + 8");
        }

        /// <summary>
        /// Tests a invalid token following an opening parantheses
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10()
        {
            Formula f = new Formula("(-)");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// This tests a simple operation enclosed in parentheses.
        /// </summary>
        [TestMethod]
        public void Evaluate6()
        {
            Formula f = new Formula("(2 - 1)");
            Assert.AreEqual(f.Evaluate(v => 0), 1, 1e-6);
        }

        /// <summary>
        /// This tests that it operates left to right with add and subtract.
        /// </summary>
        [TestMethod]
        public void Evaluate7()
        {
            Formula f = new Formula("2 - 1 + 1");
            Assert.AreEqual(f.Evaluate(v => 0), 2, 1e-6);
        }

        /// <summary>
        /// This tests a lookup of not a number.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void Evaluate8()
        {
            Formula f = new Formula("x - 1");
            Assert.AreEqual(f.Evaluate(v => double.NaN), 2, 1e-6);
        }

        /// <summary>
        /// This tests dividing by zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate9()
        {
            Formula f = new Formula("1 / 0");
            Assert.AreEqual(f.Evaluate(v => 0), 2, 1e-6);
        }

        /// <summary>
        /// Test an exception is thrown from validator.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaStruct0()
        {
            Formula f1 = new Formula("x", s => s, s => false);
        }

        /// <summary>
        /// Test a validator that returns true.
        /// </summary>
        [TestMethod]
        public void FormulaStruct1()
        {
            Formula f1 = new Formula("x", s => s, s => true);
        }

        /// <summary>
        /// Test GetVariables.
        /// </summary>
        [TestMethod]
        public void FormulaStruct2()
        {
            Formula f1 = new Formula("x2+y3+1");
            HashSet<string> vars = f1.GetVariables();

            Assert.IsTrue(vars.Contains("x2"));
            Assert.IsTrue(vars.Contains("y3"));
            Assert.IsTrue(!vars.Contains("1"));
            Assert.IsTrue(vars.Count == 2);
        }

        /// <summary>
        /// Test an exception is thrown from a complex validator.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaStruct3()
        {
            Formula f1 = new Formula("x+y3", s => s.ToUpper(), s => Regex.IsMatch(s, "^[A-Z]$"));
            f1.ToString();
        }

        /// <summary>
        /// Test GetVariables after Normalized.
        /// </summary>
        [TestMethod]
        public void FormulaStruct4()
        {
            Formula f1 = new Formula("x2", s=> s.ToUpper(), s => true);
            HashSet<string> vars = f1.GetVariables();
            Assert.IsTrue(vars.Contains("X2"));
        }

        /// <summary>
        /// Test zero argument Formula Constructor.
        /// </summary>
        [TestMethod]
        public void FormulaStruct5()
        {
            Formula f = new Formula();
            Assert.AreEqual(f.Evaluate(v => 0), 0, 1e-6);
        }

        /// <summary>
        /// Test creating a formula matching another formula.
        /// </summary>
        [TestMethod]
        public void FormulaStruct6()
        {
            Formula f1 = new Formula("1 + 1");
            Formula f2 = new Formula(f1.ToString(), s => s, s => true);
            Assert.AreEqual(f2.Evaluate(v => 0), 2, 1e-6);
            Assert.AreNotSame(f2, f1);
        }

        /// <summary>
        /// Test a valid normalizer and validator.
        /// </summary>
        [TestMethod]
        public void FormulaStruct7()
        {
            Formula f1 = new Formula("x2+y3", s => s.ToUpper(), s => Regex.IsMatch(s, "^[A-Z]+[0-9]+$"));
            Assert.AreEqual(f1.Evaluate(v =>
            {
                if (v != "X2") return 1;
                if (v != "Y3") return 3;
                return 1;
            }), 4, 1e-6);
        }

        /// <summary>
        /// Test an empty formula string in the second constructor.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaStruct8()
        {
            Formula f1 = new Formula("", s => s.ToUpper(), s => Regex.IsMatch(s, "^[A-Z]+[0-9]+$"));
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
