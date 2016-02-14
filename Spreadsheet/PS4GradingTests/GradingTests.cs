using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using Dependencies;
using System.Collections.Generic;
using System;

namespace PS4GradingTests
{
    [TestClass]
    public class GradingTests
    {
        [TestMethod]
        public void ZeroArg1()
        {
            new Formula();
        }

        [TestMethod]
        public void ZeroArg2()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.GetVariables().Count);
        }

        [TestMethod]
        public void ZeroArg3()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.Evaluate(s => 1));
        }

        [TestMethod]
        public void ZeroArg4()
        {
            Formula f1 = new Formula();
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(0, f2.Evaluate(s => 1));
        }

        [TestMethod]
        public void ThreeArg1()
        {
            Formula f = new Formula("x+y", s => s, s => true);
            Assert.AreEqual(3, f.Evaluate(s => (s == "x") ? 1 : 2));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg2()
        {
            Formula f = new Formula("x+y", s => "$", s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg3()
        {
            Formula f = new Formula("x+y", s => s, s => false);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg4()
        {
            Formula f = new Formula("x+y", s => s == "x" ? "z" : s, s => s != "z");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ThreeArg5()
        {
            Formula f = new Formula("$", s => "x", s => true);
        }

        [TestMethod]
        public void ThreeArg6()
        {
            Formula f = new Formula("1", s => "x", s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => { throw new UndefinedVariableException(""); }), 1e-6);
        }

        [TestMethod]
        public void ThreeArg7()
        {
            Formula f = new Formula("y", s => "x", s => true);
            Assert.AreEqual(1.0, f.Evaluate(s => (s == "x") ? 1 : 0), 1e-6);
        }


        [TestMethod]
        public void ThreeArg8()
        {
            Formula f = new Formula("1e1 + e", s => "x", s => s == "x");
            Assert.AreEqual(12.0, f.Evaluate(s => (s == "x") ? 2 : 0), 1e-6);
        }

        [TestMethod]
        public void ThreeArg9()
        {
            Formula f = new Formula("xx+y", s => (s == "xx") ? "X" : "z", s => s.Length == 1);
            Assert.AreEqual(10.0, f.Evaluate(s => (s == "X") ? 7 : 3), 1e-6);
        }

        [TestMethod]
        public void ThreeArg10()
        {
            Formula f = new Formula("a + b + c + d", s => "x", s => true);
            Assert.AreEqual(4.0, f.Evaluate(s => (s == "x") ? 1 : 0), 1e-6);
        }

        [TestMethod]
        public void GetVars1()
        {
            Formula f = new Formula("0");
            var expected = new HashSet<string>();
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void GetVars2()
        {
            Formula f = new Formula("x");
            var expected = new HashSet<string>();
            expected.Add("x");
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void GetVars3()
        {
            Formula f = new Formula("a * b - c + d / e * 2.5e6");
            var expected = new HashSet<string>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            expected.Add("e");
            var actual = f.GetVariables();
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void GetVars4()
        {
            Formula f = new Formula("a * a + b * c - d * d");
            var expected = new HashSet<string>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            var actual = f.GetVariables();
            Assert.AreEqual(4, actual.Count);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void GetVars5()
        {
            Formula f = new Formula("x+y", s => s.ToUpper(), s => true);
            var expected = new HashSet<string>();
            expected.Add("X");
            expected.Add("Y");
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void GetVars6()
        {
            Formula f = new Formula("x+y+z", s => s + s, s => true);
            var expected = new HashSet<string>();
            expected.Add("xx");
            expected.Add("yy");
            expected.Add("zz");
            Assert.IsTrue(expected.SetEquals(f.GetVariables()));
        }

        [TestMethod]
        public void ToString1()
        {
            Formula f1 = new Formula("7");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(7.0, f2.Evaluate(s => 0), 1e-6);
        }

        [TestMethod]
        public void ToString2()
        {
            Formula f1 = new Formula("x");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(8.0, f2.Evaluate(s => (s == "x") ? 8 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString3()
        {
            Formula f1 = new Formula("x", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(8.0, f2.Evaluate(s => (s == "X") ? 8 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString4()
        {
            Formula f1 = new Formula("a+b*(c-15)/2");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsLower(s[0]) ? 16 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString5()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", s => s, s => true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsLower(s[0]) ? 16 : 0), 1e-6);
        }

        [TestMethod]
        public void ToString6()
        {
            Formula f1 = new Formula("a+b*(c-15)/2", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(24.0, f2.Evaluate(s => char.IsUpper(s[0]) ? 16 : 0), 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null1()
        {
            DependencyGraph d = new DependencyGraph();
            d.AddDependency("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null2()
        {
            DependencyGraph d = new DependencyGraph();
            d.HasDependees(null);
        }

        [TestMethod]
        public void Copy1()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            Assert.AreEqual(0, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy2()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy3()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "b");
            d2.AddDependency("c", "d");
            Assert.IsTrue(d1.HasDependents("a"));
            Assert.IsFalse(d1.HasDependents("c"));
            Assert.IsFalse(d2.HasDependents("a"));
            Assert.IsTrue(d2.HasDependents("c"));
        }

        [TestMethod]
        public void Copy4()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            var d2 = new DependencyGraph(d1);
            Assert.IsTrue(d1.HasDependees("b"));
            Assert.IsTrue(d2.HasDependees("b"));
        }

        [TestMethod]
        public void Copy5()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            d1.AddDependency("d", "e");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "c");
            d2.AddDependency("d", "f");
            Assert.AreEqual(2, new List<string>(d1.GetDependents("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependents("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependents("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependents("a")).Count);
        }

        [TestMethod]
        public void Copy6()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("b", "a");
            d1.AddDependency("e", "d");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("c", "a");
            d2.AddDependency("f", "d");
            Assert.AreEqual(2, new List<string>(d1.GetDependees("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependees("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependees("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependees("a")).Count);
        }

        [TestMethod]
        public void Copy7()
        {
            var d1 = new DependencyGraph();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    d1.AddDependency(i.ToString(), j.ToString());
                }
            }
            var d2 = new DependencyGraph(d1);

            for (int i = 0; i < 100; i++)
            {
                d1.RemoveDependency(i.ToString(), i.ToString());
                d2.AddDependency(i.ToString(), "x");
            }

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, new List<string>(d1.GetDependents(i.ToString())).Count);
                Assert.AreEqual(i + 2, new List<string>(d2.GetDependents(i.ToString())).Count);
            }

            for (int j = 0; j <= 50; j++)
            {
                Assert.AreEqual(99 - j, new List<string>(d1.GetDependees(j.ToString())).Count);
                Assert.AreEqual(100 - j, new List<string>(d2.GetDependees(j.ToString())).Count);
            }

            Assert.AreEqual(100, new List<string>(d2.GetDependees("x")).Count);

            Assert.AreEqual(5050 - 100, d1.Size);
            Assert.AreEqual(5050 + 100, d2.Size);
        }

        [TestMethod]
        public void Copy8()
        {
            Copy7();
        }
    }
}
