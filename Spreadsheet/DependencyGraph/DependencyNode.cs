using System.Collections.Generic;
using System.Linq;

namespace Dependencies
{
    /// <summary>
    /// A Dependency Node belongs to a Dependency Graph. 
    /// A node an abstraction that contains the links to its dependees and dependents.
    /// </summary>
    public class DependencyNode
    {
        private string _key;
        private Dictionary<string, DependencyNode> _linkedDependency = new Dictionary<string, DependencyNode>();
        private Dictionary<string, DependencyNode> _linkedDependees = new Dictionary<string, DependencyNode>();

        /// <summary>
        /// Constructor for a DependencyNode.
        /// </summary>
        /// <param name="key">A string to associate the node by.</param>
        public DependencyNode(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Returns the count of dependents for the node.
        /// </summary>
        public int CountDependents()
        {
            return _linkedDependency.Count;
        }

        /// <summary>
        /// Returns the count of dependees for the node.
        /// </summary>
        public int CountDependees()
        {
            return _linkedDependees.Count;
        }

        /// <summary>
        /// Remove the link between all dependents and the node.
        /// </summary>
        public void ClearDependents()
        {
            foreach (var dependent in _linkedDependency.Values)
            {
                dependent.RemoveDependee(this);
            }
            _linkedDependency.Clear();
        }

        /// <summary>
        /// Removes the link between all dependees and the node.
        /// </summary>
        public void ClearDependees()
        {
            foreach (var dependee in _linkedDependees.Values)
            {
                dependee.RemoveDependency(this);
            }
            _linkedDependees.Clear();
        }

        /// <summary>
        /// Returns true if the node has the dependency of tNode.
        /// </summary>
        public bool HasDependency(DependencyNode tNode)
        {
            return _linkedDependency.ContainsKey(tNode.ToString());
        }

        /// <summary>
        /// Returns true if the node has the dependee of sNode.
        /// </summary>
        public bool HasDependee(DependencyNode sNode)
        {
            return _linkedDependees.ContainsKey(sNode.ToString());
        }

        /// <summary>
        /// Returns the Dependents of the node.
        /// </summary>
        public IEnumerable<string> GetDependents()
        {
            return _linkedDependency.Values.Select(node => node.ToString());
        }

        /// <summary>
        /// Returns the Dependees of the node.
        /// </summary>
        public IEnumerable<string> GetDependees()
        {
            return _linkedDependees.Values.Select(node => node.ToString());
        }

        /// <summary>
        /// Adds the linkedNode as a dependency of the node.
        /// </summary>
        public void AddDependency(DependencyNode linkedNode)
        {
            if (HasDependency(linkedNode)) return; //Return if the link already exists.
            _linkedDependency.Add(linkedNode.ToString(), linkedNode);
        }

        /// <summary>
        /// Removes the linkedNode as a dependency of the node.
        /// </summary>
        public void RemoveDependency(DependencyNode linkedNode)
        {
            if (_linkedDependency.ContainsKey(linkedNode.ToString()))
            {
                _linkedDependency.Remove(linkedNode.ToString());
            }
        }

        /// <summary>
        /// Adds the linkedNode as a dependee of the node.
        /// </summary>
        public void AddDependee(DependencyNode linkedNode)
        {
            if (HasDependee(linkedNode)) return; //Return if the link already exists.
            _linkedDependees.Add(linkedNode.ToString(), linkedNode);
        }

        /// <summary>
        /// Removes the linkedNode as a dependee of the node.
        /// </summary>
        public void RemoveDependee(DependencyNode linkedNode)
        {
            if (_linkedDependees.ContainsKey(linkedNode.ToString()))
            {
                _linkedDependees.Remove(linkedNode.ToString());
            }
        }

        /// <summary>
        /// Returns the key of the node.
        /// </summary>
        public override string ToString()
        {
            return _key;
        }
    }
}
