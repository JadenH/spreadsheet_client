using System.Collections.Generic;
using Formulas;

namespace SS
{
    /// <summary>
    /// Simple cell struct that contains a cell's contents.
    /// </summary>
    public struct Cell
    {
        private object _cellContents;
        private double? _value;

        /// <summary>
        /// Cell constructor for a formula.
        /// </summary>
        public Cell(Formula cellContents, Dictionary<string, Cell> cells)
        {
            _cellContents = cellContents;
            _value = cellContents.Evaluate(c =>
            {
                if (!cells.ContainsKey(c)) return 0;
                return (double) cells[c].GetValue(cells);
            });
        }

        /// <summary>
        /// Cell constructor for a double.
        /// </summary>
        public Cell(double cellContents)
        {
            _cellContents = cellContents;
            _value = cellContents;
        }

        /// <summary>
        /// Cell constructor for a string.
        /// </summary>
        public Cell(string cellContents)
        {
            _cellContents = cellContents;
            _value = null;
        }

        /// <summary>
        /// Returns the contents of the cell (not the value).
        /// </summary>
        public object GetCellContents()
        {
            return _cellContents;
        }

        /// <summary>
        /// Returns the cell value.
        /// </summary>
        public object GetValue(Dictionary<string, Cell> cells)
        {
            if (!(_cellContents is Formula)) return _cellContents;
            Formula cellContents = (Formula)_cellContents;
            return _value ?? (_value = cellContents.Evaluate(c =>
            {
                if (!cells.ContainsKey(c)) return 0;
                return (double) cells[c].GetValue(cells);
            }));
        }

    }
}
