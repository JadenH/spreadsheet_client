using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SpreadsheetGUI.Properties;
using SS;

namespace SpreadsheetGUI
{
    public class Controller
    {
        protected ISpreadsheetView _window;

        private Spreadsheet _spreadsheet;
        public GuiCell _selectedCell;
        private string _spreadsheetName = "New Spreadsheet";
        private string _savePath;

        /// <summary>
        /// Initializes a controller for the given window. 
        /// This is the controlling component in the MVC framework.
        /// </summary>
        /// <param name="window"></param>
        public Controller(ISpreadsheetView window)
        {
            _window = window;
            _selectedCell = new GuiCell(0, 0);
            _spreadsheet = new Spreadsheet();

            //Event Subscriptions
            _window.CellValueBoxTextChange += CellValueBarChanged;
            _window.CellSelectionChange += SpreadsheetSelectionChanged;
            _window.CreateNew += CreateNew;
            _window.HandleOpen += HandleOpen;
            _window.HandleSave += () => HandleSave(_savePath);
            _window.HandleSaveAs += () => HandleSave(null);
            _window.HandleClose += HandleClose;
            _window.HandleHelp += WindowOnHandleHelp;

            //Setup defaults
            _window.SetSelection(_selectedCell.CellColumn, _selectedCell.CellRow);
            UpdateInfoBar($"{_selectedCell.CellName}: { _selectedCell.GetCellValue(_spreadsheet)}", Color.White);
            UpdateCellNameText();
        }

        /// <summary>
        /// Creates a message box containing simple instructions about using the spreadsheet application.
        /// </summary>
        public void WindowOnHandleHelp()
        {
            //References the Resources in the project.
            MessageBox.Show(Resources.HelpInfo, @"Help",
                    MessageBoxButtons.OK);
        }

        /// <summary>
        /// Changes the current window/controller to a given spreadsheet and updates the view accordingly.
        /// </summary>
        /// <param name="spreadsheet"></param>
        public void ChangeSpreadsheet(Spreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            UpdateCells();
        }

        /// <summary>
        /// Handles closing of the document/application. If the document has unsaved changes the user will be prompted
        /// whether or not they would like to save the given document before closing.
        /// </summary>
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

        /// <summary>
        /// Creates a save window dialog for the user to select a path in which to save the file to.
        /// </summary>
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

        /// <summary>
        /// Saves the spreadsheet to the given path in the parameter. 
        /// </summary>
        private void HandleSave(string path)
        {
            //If we don't have a path to save to we should get one from a save dialog.
            if (path == null) path = SaveWindow();
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
                MessageBox.Show("File was not saved.\nFile name or directory invalid.", @"Error Saving File");
            }
        }

        /// <summary>
        /// Handles opening a file and creates a new window with the spreadsheet being opened.
        /// If the spreadsheet fails to open a MessageBox is displayed with an appropriate error message.
        /// </summary>
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

        /// <summary>
        /// Handles creating a new document.
        /// </summary>
        public void CreateNew()
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
            UpdateInfoBar($"{_selectedCell.CellName}: { _selectedCell.GetCellValue(_spreadsheet)}", Color.White);
            UpdateCellNameText();
        }

        /// <summary>
        /// Sets the selected cell contents to the new value from the data bar. If an exception occurs the info bar will
        /// display an error message.
        /// </summary>
        private void CellValueBarChanged(string value)
        {
            //Set the info bar to be empty.
            _window.InfoBarText = string.Empty;
            try
            {
                UpdateCells(_spreadsheet.SetContentsOfCell(_selectedCell.CellName, value));
                UpdateInfoBar($"{_selectedCell.CellName}: { _selectedCell.GetCellValue(_spreadsheet)}", Color.White);
                _window.SetTitle(_spreadsheetName + "*");
            }
            catch (Exception e)
            {
                UpdateInfoBar(e.Message, Color.Red);
            }


        }

        /// <summary>
        /// Iterates through the given Hashset and updates the cells in the view.
        /// </summary>
        /// <param name="cellNames"></param>
        private void UpdateCells(IEnumerable<string> cellNames)
        {
            foreach (var cellName in cellNames)
            {
                GuiCell cell = new GuiCell(cellName);
                _window.UpdateCell(cell.CellColumn, cell.CellRow, cell.GetCellValue(_spreadsheet));
            }
        }

        /// <summary>
        /// Updates the cells in the view for all nonempty cells in the spreadsheet.
        /// </summary>
        private void UpdateCells()
        {
            UpdateCells(_spreadsheet.GetNamesOfAllNonemptyCells());
        }

        /// <summary>
        /// Updates the Infobar with the given text and color.
        /// </summary>
        private void UpdateInfoBar(string text, Color color)
        {
            _window.InfoBarText = text;
            _window.InfoBarColor = color;
        }

        /// <summary>
        /// Updates the cellname text next to the cell value bar with the current selected cell.
        /// </summary>
        private void UpdateCellNameText()
        {
            _window.SelectedCellText = _selectedCell.CellName;
        }

        /// <summary>
        /// Sets the save path for the spreadsheet that is open. This will also update the window title.
        /// </summary>
        /// <param name="path"></param>
        public void SetSavedPath(string path)
        {
            _spreadsheetName = Path.GetFileNameWithoutExtension(path);
            _savePath = path;
            _window.SetTitle(_spreadsheetName);
        }
    }
}