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
    /// 
    /// </summary>
    public struct Cell
    {
        /// <summary>
        /// 
        /// </summary>
        public object CellContents { get; }

        /// <summary>
        /// 
        /// </summary>
        public Cell(object cellContents)
        {
            CellContents = cellContents;
        }

        /// <summary>
        /// 
        /// </summary>
        public object GetValue()
        {
            if (CellContents == null) return string.Empty;
            if (CellContents is Formula)
            {
                Formula cellContents = (Formula) CellContents;
                return cellContents.Evaluate(v => 0);
            }
            if (CellContents is double)
            {
                return (double) CellContents;
            }
            return CellContents as string;
        }

    }
}
