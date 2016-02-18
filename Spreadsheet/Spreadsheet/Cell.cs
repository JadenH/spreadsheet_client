using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;

namespace SS
{
    /// <summary>
    /// Simple cell struct that contains a cell's contents.
    /// </summary>
    public struct Cell
    {
        private object _cellContents;

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
        /// Returns the cell value.
        /// </summary>
        public object GetValue()
        {
            if (!(_cellContents is Formula)) return _cellContents;
            Formula cellContents = (Formula) _cellContents;
            return cellContents.Evaluate(v => 0);
        }

    }
}
