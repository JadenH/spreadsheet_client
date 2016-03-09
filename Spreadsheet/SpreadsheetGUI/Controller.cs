using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SS;
using SSGui;

namespace SpreadsheetGUI
{
    public class Controller
    {
        private ISpreadsheetView _window;

        private Spreadsheet _spreadsheet;
        private GuiCell _selectedCell;
        private string _spreadsheetName = "New Spreadsheet";
        private string _savePath;

        public Controller(ISpreadsheetView window)
        {
            _window = window;
            _selectedCell = new GuiCell(0, 0);
            _spreadsheet = new Spreadsheet();

            //Event Subscriptions
            _window.CellValueBoxTextChange += DataBarChanged;
            _window.CellSelectionChange += SpreadsheetSelectionChanged;
            _window.CreateNew += CreateNew;
            _window.HandleOpen += HandleOpen;
            _window.HandleSave += () => HandleSave(null);
            _window.HandleSaveAs += () => HandleSave(_savePath);
            _window.HandleClose += HandleClose;

            //Setup defaults
            _window.SetSelection(0, 0);
            UpdateDataBar();
            UpdateCellNameText();
        }

        public void ChangeSpreadsheet(Spreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            UpdateCells();
        }

        private bool HandleClose()
        {
            if (_spreadsheet.Changed)
            {
                var response = MessageBox.Show($"Want to save your changes for {_spreadsheetName} before closing?", @"Spreadsheet",
                    MessageBoxButtons.YesNoCancel);
                switch (response)
                {
                    case DialogResult.Yes:
                        HandleSave(_savePath);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                    case DialogResult.Abort:
                        return true;
                }
            }
            return false;
        }

        private string SaveWindow()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = false,
                CheckPathExists = true,
                Filter = @"Spreadsheet (*.ss)|*.ss|All files (*.*)|*.*"
            };

            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                bool extension = saveFileDialog.FilterIndex == 1;
                string filePath = saveFileDialog.FileName;
                if (extension && Path.GetExtension(filePath) != ".ss") filePath += ".ss";
                return filePath;
            }
            return null;
        }

        private void HandleSave(string path)
        {
            if (_savePath == null) path = SaveWindow();
            if (path != null)
            {
                try
                {
                    TextWriter textWriter = new StreamWriter(path);
                    _spreadsheet.Save(textWriter);
                    textWriter.Close();
                    SetSavedPath(path);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occured saving the file.\n{e.Message}", @"Error Saving File");
                }
            }
            else
            {
                MessageBox.Show(@"File was not saved.\nFile name or directory invalid.", @"Error Saving File");
            }
        }

        private void HandleOpen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = @"Spreadsheet (.ss)|*.ss|All Files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    SpreadsheetApplicationContext.GetContext().RunNew(openFileDialog.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error occured opening the file.\n{e.Message}", @"Error Opening File");
                }
            }
        }

        private void CreateNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }


        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter.
        /// </summary>
        private void SpreadsheetSelectionChanged(int col, int row)
        {
            _selectedCell = new GuiCell(col, row);
            _window.CellValueBoxText = _selectedCell.GetCellContents(_spreadsheet);
            UpdateDataBar();
            UpdateCellNameText();
        }

        private void DataBarChanged(string value)
        {
            _window.InfoBarText = string.Empty;
            try
            {
                HashSet<string> recalculatedCells = _spreadsheet.SetContentsOfCell(_selectedCell.CellName, value) as HashSet<string>;
                UpdateCells(recalculatedCells);
                UpdateDataBar();
            }
            catch (Exception e)
            {
                _window.InfoBarText = e.Message;
                _window.InfoBarColor = Color.Red;
            }

        }

        private void UpdateCells(HashSet<string> cellNames)
        {
            foreach (var cellName in cellNames)
            {
                GuiCell cell = new GuiCell(cellName);
                _window.UpdateCell(cell.CellColumn, cell.CellRow, cell.GetCellValue(_spreadsheet));
            }
        }

        private void UpdateCells()
        {
            HashSet<string> cellsToUpdate = new HashSet<string>(_spreadsheet.GetNamesOfAllNonemptyCells());
            UpdateCells(cellsToUpdate);
        }

        private void UpdateDataBar()
        {
            _window.InfoBarText = $"{_selectedCell.CellName}: { _selectedCell.GetCellValue(_spreadsheet)}";
            _window.InfoBarColor = Color.White;
        }

        private void UpdateCellNameText()
        {
            _window.SelectedCellText = _selectedCell.CellName;
        }

        public void SetSavedPath(string path)
        {
            _spreadsheetName = Path.GetFileNameWithoutExtension(path);
            _savePath = path;
            _window.SetTitle(_spreadsheetName);
        }
    }
}