﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SpreadsheetGUI.Properties;
using SS;

namespace SpreadsheetGUI
{
    public partial class Controller
    {
        protected ISpreadsheetView _window;
        protected ISpreadsheetServer _server;

        public Spreadsheet Spreadsheet;
        public GuiCell SelectedCell;

        private string _spreadsheetName = "New Spreadsheet";
        private string _savePath;

        /// <summary>
        /// Initializes a controller for the given window. 
        /// This is the controlling component in the MVC framework.
        /// </summary>
        /// <param name="window"></param>
        public Controller(ISpreadsheetView window, ISpreadsheetServer server)
        {
            _window = window;
            _server = server;
            SelectedCell = new GuiCell(0, 0);
            Spreadsheet = new Spreadsheet();

            // Event Subscriptions
            _window.CellValueBoxTextComplete += CellValueBarChanged;
            _window.CellSelectionChange += SpreadsheetSelectionChanged;
            _window.CreateNew += CreateNew;
            _window.HandleOpen += () => HandleOpen(null);
            _window.HandleSave += () => HandleSave(_savePath);
            _window.HandleSaveAs += () => HandleSave(null);
            _window.HandleClose += HandleClose;
            _window.HandleHelp += WindowOnHandleHelp;
            _window.HandleUndo += Undo;

            // Server Subscriptions
            _server.MessageReceived += MessageReceived;

            //Setup defaults
            _window.SetSelection(SelectedCell.CellColumn, SelectedCell.CellRow);
            UpdateInfoBar($"{SelectedCell.CellName}: { SelectedCell.GetCellValue(Spreadsheet)}", Color.White);
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
            Spreadsheet = spreadsheet;
            UpdateCells();
        }

        /// <summary>
        /// Handles closing of the document/application. If the document has unsaved changes the user will be prompted
        /// whether or not they would like to save the given document before closing.
        /// </summary>
        private bool HandleClose()
        {
            return false;
        }

        /// <summary>
        /// Saves the spreadsheet to the given path in the parameter. 
        /// </summary>
        public void HandleSave(string path)
        {
        }

        /// <summary>
        /// Handles opening a file and creates a new window with the spreadsheet being opened.
        /// If the spreadsheet fails to open a MessageBox is displayed with an appropriate error message.
        /// </summary>
        public void HandleOpen(string path)
        {
            SpreadsheetApplicationContext.GetContext().RunLauncher();
        }

        /// <summary>
        /// Handles creating a new document.
        /// </summary>
        public void CreateNew()
        {
            SpreadsheetApplicationContext.GetContext().RunLauncher();
        }

        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter.
        /// </summary>
        private void SpreadsheetSelectionChanged(int col, int row)
        {
            SelectedCell = new GuiCell(col, row);
            _window.CellValueBoxText = SelectedCell.GetCellContents(Spreadsheet);
            UpdateInfoBar($"{SelectedCell.CellName}: { SelectedCell.GetCellValue(Spreadsheet)}", Color.White);
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
                SendCell(SelectedCell.CellName, value);
                UpdateInfoBar($"{SelectedCell.CellName}: { SelectedCell.GetCellValue(Spreadsheet)}", Color.White);
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
                _window.UpdateCell(cell.CellColumn, cell.CellRow, cell.GetCellValue(Spreadsheet));
                if (SelectedCell.CellName == cell.CellName)
                {
                    _window.CellValueBoxText = SelectedCell.GetCellContents(Spreadsheet);
                }
            }
        }

        /// <summary>
        /// Updates the cells in the view for all nonempty cells in the spreadsheet.
        /// </summary>
        private void UpdateCells()
        {
            UpdateCells(Spreadsheet.GetNamesOfAllNonemptyCells());
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
            _window.SelectedCellText = SelectedCell.CellName;
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