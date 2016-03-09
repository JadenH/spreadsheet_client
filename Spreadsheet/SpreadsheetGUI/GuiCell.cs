using System;
using System.Linq;
using System.Text.RegularExpressions;
using Formulas;
using SS;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Simple struct that will take a cell name (A1) and convert to a cell row/column. Also takes a cell column/row
    /// and converts it to a cell name (A1). Given a spreadsheet, it will return a value.
    /// </summary>
    struct GuiCell
    {
        public string CellName;
        public int CellColumn;
        public int CellRow;
        const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Takes a row and a column and will convert to a cell name.
        /// </summary>
        public GuiCell(int column, int row)
        {
            //Setup the struct
            CellName = string.Empty;
            CellColumn = column;
            CellRow = row;

            //Set the cell name.
            CellName = GetCellName(column, row);
        }

        /// <summary>
        /// Takes a cellname and will convert to row/col.
        /// </summary>
        /// <param name="cellName"></param>
        public GuiCell(string cellName)
        {
            //Setup the struct
            CellName = cellName;
            CellColumn = 0;
            CellRow = 0;

            //Set the cell row and column.
            GetCellIndex(cellName);
        }

        /// <summary>
        /// Returns the cell name.
        /// </summary>
        private string GetCellName(int column, int row)
        {
            string cellName = string.Empty;
            while (column >= 0)
            {
                cellName += Letters[column % 26];
                column -= 26;
            }
            cellName += row + 1;
            return cellName;
        }

        /// <summary>
        /// Calculates the cells row and column given a cell name.
        /// </summary>
        private void GetCellIndex(string CellName)
        {
            Match rowMatch = new Regex("([0-9]+)").Match(CellName);
            foreach (char letter in CellName)
            {
                if (char.IsLetter(letter)) CellColumn += letter - 65;
            }
            CellRow = int.Parse(rowMatch.Groups[0].Value)-1;
        }

        /// <summary>
        /// Returns the cell value from a given spreadsheet.
        /// </summary>
        public string GetCellValue(Spreadsheet spreadsheet)
        {
            var cellValue = spreadsheet.GetCellValue(CellName);
            if (cellValue is FormulaError)
            {
                FormulaError error = (FormulaError) cellValue;
                return error.Reason;
            }
            return cellValue.ToString();
        }

        /// <summary>
        /// Returns the cell contents given a spreadsheet.
        /// </summary>
        public string GetCellContents(Spreadsheet spreadsheet)
        {
            var value = spreadsheet.GetCellContents(CellName);
            if (value is Formula)
            {
                return "=" + value;
            }
            if (value is FormulaError)
            {
                return "ERROR";
            }
            return value.ToString();
        }

    }
}
