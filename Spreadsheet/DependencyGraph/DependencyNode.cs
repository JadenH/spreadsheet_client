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

        public DependencyNode(string key)
        {
            _key = key;
        }

        public int CountDependents()
        {
            return _linkedDependency.Count;
        }

        public int CountDependees()
        {
            return _linkedDependees.Count;
        }

        public void ClearDependents()
        {
            foreach (var dependent in _linkedDependency.Values)
            {
                dependent.RemoveDependee(this);
            }
            _linkedDependency.Clear();
        }

        public void ClearDependees()
        {
            foreach (var dependee in _linkedDependees.Values)
            {
                dependee.RemoveDependency(this);
            }
            _linkedDependees.Clear();
        }

        public bool HasDependency(DependencyNode tNode)
        {
            return _linkedDependency.ContainsKey(tNode.ToString());
        }

        public bool HasDependee(DependencyNode sNode)
        {
            return _linkedDependees.ContainsKey(sNode.ToString());
        }

        public IEnumerable<string> GetDependents()
        {
            return _linkedDependency.Values.Select(node => node.ToString());
        }

        public IEnumerable<string> GetDependees()
        {
            return _linkedDependees.Values.Select(node => node.ToString());
        }

        public void AddDependency(DependencyNode linkedNode)
        {
            if (HasDependency(linkedNode)) return; //Return if the link already exists.
            _linkedDependency.Add(linkedNode.ToString(), linkedNode);
        }

        public void RemoveDependency(DependencyNode linkedNode)
        {
            if (_linkedDependency.ContainsKey(linkedNode.ToString()))
            {
                _linkedDependency.Remove(linkedNode.ToString());
            }
        }

        public void AddDependee(DependencyNode linkedNode)
        {
            if (HasDependee(linkedNode)) return; //Return if the link already exists.
            _linkedDependees.Add(linkedNode.ToString(), linkedNode);
        }

        public void RemoveDependee(DependencyNode linkedNode)
        {
            if (_linkedDependees.ContainsKey(linkedNode.ToString()))
            {
                _linkedDependees.Remove(linkedNode.ToString());
            }
        }

        public override string ToString()
        {
            return _key;
        }
    }
}
