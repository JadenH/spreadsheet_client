using System;
using System.Collections.Generic;
using System.Linq;
using Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyGraphTests
{
    [TestClass]
    public class DependencyGraphTests
    {

        /// <summary>
        /// Test creating a Dependency Graph.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsInstanceOfType(graph, typeof(DependencyGraph));
        }

        /// <summary>
        /// Test adding/getting a Dependency.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            string s = graph.GetDependents("a").First();
            Assert.AreEqual(s, "b");
        }

        /// <summary>
        /// Test adding/getting a Dependees.
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            Assert.AreEqual(graph.GetDependees("b").First(), "a");
        }

        /// <summary>
        /// Test has dependents to be true.
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            Assert.IsTrue(graph.HasDependents("a"));
        }

        /// <summary>
        /// Test has dependents to be false.
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsFalse(graph.HasDependents("a"));
        }

        /// <summary>
        /// Test has dependents to be true.
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            Assert.IsTrue(graph.HasDependents("a"));
        }

        /// <summary>
        /// Test has dependees to be true.
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            Assert.IsTrue(graph.HasDependees("b"));
        }

        /// <summary>
        /// Test has dependees to be true.
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsFalse(graph.HasDependees("a"));
        }

        /// <summary>
        /// Test removing dependecies.
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.RemoveDependency("a", "b");
            Assert.IsFalse(graph.HasDependees("a"));
        }

        /// <summary>
        /// Test replace dependencies.
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.ReplaceDependents("a", new[] { "c", "d" });
            string[] dependents = graph.GetDependents("a").ToArray();
            Assert.IsTrue(dependents.Contains("c"));
            Assert.IsTrue(dependents.Contains("d"));
        }

        /// <summary>
        /// Test replace dependees.
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.ReplaceDependees("b", new[] { "c", "d" });
            string[] dependees = graph.GetDependees("b").ToArray();
            Assert.IsTrue(dependees.Contains("c"));
            Assert.IsTrue(dependees.Contains("d"));
        }

        /// <summary>
        /// Test Graph size
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            Assert.AreEqual(graph.Size, 1);
        }

        /// <summary>
        /// Test Null value in AddDependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod13()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency(null, null);
        }

        /// <summary>
        /// Test Null value in RemoveDependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod14()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.RemoveDependency(null, null);
        }

        /// <summary>
        /// Test Null value in GetDependees
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod15()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsTrue(graph.HasDependees(null));
        }

        /// <summary>
        /// Test Null value in HasDependents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethod16()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsTrue(graph.HasDependents(null));
        }

        /// <summary>
        /// Test adding a lot of Dependents.
        /// </summary>
        [TestMethod]
        public void TestMethod17()
        {
            DependencyGraph graph = new DependencyGraph();
            for (int i = 1000; i > 0; i--)
            {
                graph.AddDependency($"a{i}", $"b{i}");
                Assert.IsTrue(graph.HasDependents($"a{i}"));
                Assert.AreEqual(graph.GetDependents($"a{i}").First(), $"b{i}");
            }
        }

        /// <summary>
        /// Test adding a dependency twice.
        /// </summary>
        [TestMethod]
        public void TestMethod18()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "b");
            Assert.AreEqual(graph.GetDependents("a").Count(), 1);
        }

        /// <summary>
        /// Test replacing dependees.
        /// </summary>
        [TestMethod]
        public void TestMethod19()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.ReplaceDependees("b", new List<string> { "d" });
            Assert.IsFalse(graph.GetDependents("a").Contains("b"));
            Assert.IsTrue(graph.GetDependents("d").Contains("b"));
        }

        /// <summary>
        /// Test replacing dependents.
        /// </summary>
        [TestMethod]
        public void TestMethod20()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.ReplaceDependents("a", new List<string> { "d" });
            Assert.IsFalse(graph.GetDependents("a").Contains("b"));
            Assert.IsTrue(graph.GetDependents("a").Contains("d"));
        }

        /// <summary>
        /// Test getting dependents from an empty graph.
        /// </summary>
        [TestMethod]
        public void TestMethod21()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsFalse(graph.GetDependents("a").Any());
        }

        /// <summary>
        /// Test getting dependees from an empty graph.
        /// </summary>
        [TestMethod]
        public void TestMethod22()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.IsFalse(graph.GetDependees("a").Any());
        }

        /// <summary>
        /// Test adding a dependency multiple times.
        /// </summary>
        [TestMethod]
        public void TestMethod23()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "b");
            Assert.AreEqual(graph.GetDependents("a").Count(), 1);
        }

        /// <summary>
        /// Test replacing dependents with multiple of the same
        /// </summary>
        [TestMethod]
        public void TestMethod24()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "c");
            graph.ReplaceDependents("a", new List<string> { "b", "b", "b" });
            Assert.AreEqual(graph.GetDependents("a").Count(), 1);
            Assert.IsTrue(graph.GetDependents("a").Contains("b"));
        }

        /// <summary>
        /// Test replacing dependees with multiple of the same
        /// </summary>
        [TestMethod]
        public void TestMethod25()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.ReplaceDependees("b", new List<string> { "c", "c", "c" });
            Assert.AreEqual(graph.GetDependees("b").Count(), 1);
            Assert.IsTrue(graph.GetDependees("b").Contains("c"));
        }
    }
}
