// Skeleton implementation written by Joe Zachary for CS 3500, January 2015.
// Revised for CS 3500 by Joe Zachary, January 29, 2016

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, DependencyNode> _nodes = new Dictionary<string, DependencyNode>();

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
        }

        /// <summary>
        /// Creates a new DependencyGraph matching another existing Dependency Graph.
        /// </summary>
        public DependencyGraph(DependencyGraph dg)
        {
            foreach (var node in dg._nodes)
            {
                foreach (var dependent in node.Value.GetDependents())
                {
                    AddDependency(node.Key, dependent);
                }
            }
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size => _nodes.Values.Sum(node => node.CountDependents());

        private DependencyNode GetDependencyNode(string s)
        {
            if (_nodes.ContainsKey(s))
            {
                return _nodes[s];
            }
            _nodes.Add(s, new DependencyNode(s));
            return _nodes[s];
        }

        /// <summary>
        /// Removes the dependency node if it is no longer being used.
        /// </summary>
        private void CleanUp(DependencyNode node)
        {
            if (node.CountDependents() == 0 && node.CountDependees() == 0)
            {
                _nodes.Remove(node.ToString());
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty. Requires s != null. 
        /// Throws an ArgumentNullException if s is null.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (!_nodes.ContainsKey(s)) return false; //There is no such node in the graph.

            return GetDependencyNode(s).CountDependents() > 0;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty. Requires s != null. 
        /// Throws an ArgumentNullException if s is null.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (!_nodes.ContainsKey(s)) return false; //There is no such node in the graph.

            return GetDependencyNode(s).CountDependees() > 0;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null. 
        /// Throws an ArgumentNullException if s is null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (!_nodes.ContainsKey(s)) yield break; //There is no such node in the graph.

            DependencyNode sNode = GetDependencyNode(s);
            foreach (var dependent in sNode.GetDependents())
            {
                yield return dependent;
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// Throws an ArgumentNullException if s is null..
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (!_nodes.ContainsKey(s)) yield break; //There is no such node in the graph.

            DependencyNode tNode = GetDependencyNode(s);
            foreach (var dependee in tNode.GetDependees())
            {
                yield return dependee;
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null. Throws an ArgumentNullException if s or t are null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (t == null) throw new ArgumentNullException(nameof(t));
            DependencyNode sNode = GetDependencyNode(s);
            DependencyNode tNode = GetDependencyNode(t);

            //Return if we have already added this dependency.
            if (sNode.HasDependency(tNode)) return;

            //Add the dependency and increase the size of our graph.
            sNode.AddDependency(GetDependencyNode(t));
            tNode.AddDependee(GetDependencyNode(s));
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null. Throws an ArgumentNullException if s or t are null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (!_nodes.ContainsKey(s)) return; //If the node doesn't exist then we don't need to do anything.
            if (!_nodes.ContainsKey(t)) return; //If the node doesn't exist then we don't need to do anything.

            DependencyNode sNode = GetDependencyNode(s);
            DependencyNode tNode = GetDependencyNode(t);
            tNode.RemoveDependee(sNode);
            sNode.RemoveDependency(tNode);
            CleanUp(sNode);
            CleanUp(tNode);
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null. Throws an ArgumentNullException if s or t are null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            DependencyNode sNode = GetDependencyNode(s);
            sNode.ClearDependents();
            foreach (var newDependent in newDependents)
            {
                AddDependency(sNode.ToString(), newDependent);
            }
            CleanUp(sNode);
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null. Throws an ArgumentNullException if s or t are null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));

            DependencyNode tNode = GetDependencyNode(t);
            tNode.ClearDependees();
            foreach (var newDependee in newDependees)
            {
                AddDependency(newDependee, tNode.ToString());
            }
            CleanUp(tNode);
        }
    }
}
