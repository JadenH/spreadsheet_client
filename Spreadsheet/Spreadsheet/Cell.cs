using System;
using System.Collections.Generic;
using Formulas;

namespace SS
{
    /// <summary>
    /// Simple cell struct that contains a cell's contents.
    /// </summary>
    public class Cell
    {
        private object _cellContents;
        private double? _value;

        /// <summary>
        /// Cell constructor for a formula.
        /// </summary>
        public Cell(Formula cellContents)
        {
            _cellContents = cellContents;
        }

        /// <summary>
        /// Cell constructor for a double.
        /// </summary>
        public Cell(double cellContents)
        {
            _cellContents = cellContents;
        }

        /// <summary>
        /// Cell constructor for a string.
        /// </summary>
        public Cell(string cellContents)
        {
            _cellContents = cellContents;
        }

        /// <summary>
        /// Returns the contents of the cell (not the value).
        /// </summary>
        public object GetCellContents()
        {
            return _cellContents;
        }

        /// <summary>
        /// Recalculates the value of a given cell.
        /// </summary>
        public void Recalculate(Dictionary<string, Cell> cells)
        {
            _value = null;
        }

        /// <summary>
        /// Returns the cell value.
        /// </summary>
        public object GetValue(Dictionary<string, Cell> cells)
        {
            if (!(_cellContents is Formula)) return _cellContents;
            if (_value != null) return _value;
            Formula cellContents = (Formula)_cellContents;
            try
            {
                _value = cellContents.Evaluate(c => (double) cells[c].GetValue(cells));
            }
            catch (Exception e)
            {
                return new FormulaError(e.Message);
            }
            return _value;
        }

    }
}
