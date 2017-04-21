using System;
using System.Collections.Generic;
using Dependencies;
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
        public object GetValue(Dictionary<string, Cell> cells, ISet<string> visited)
        {
            if (!(_cellContents is Formula)) return _cellContents;
            if (_value != null) return _value;

            Formula cellContents = (Formula)_cellContents;
            try
            {
                _value = cellContents.Evaluate(c =>
                {
                    if (visited.Contains(c)) throw new CircularException($"Circular Dependency");
                    visited.Add(c);
                    if (!cells.ContainsKey(c)) throw new UndefinedVariableException($"The value of cell {c} is not set.");
                    var value = cells[c].GetValue(cells, visited);
                    if (value is FormulaError) throw new UndefinedVariableException(((FormulaError)value).Reason);
                    if (value is string) throw new FormulaEvaluationException($"The value of cell {c} does not contain a number or formula.");
                    return (double)value;
                });
            }
            catch (Exception e)
            {
                return new FormulaError(e.Message);
            }
            return _value;
        }

    }
}
