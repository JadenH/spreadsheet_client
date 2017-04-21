using System;
using System.ComponentModel;
using System.Drawing;

namespace SpreadsheetGUI
{
    public interface ISpreadsheetView
    {
        event Func<bool> HandleClose;
        event Action<string> CellValueBoxTextComplete;
        event Action<int, int> CellSelectionChange;
        event Action CreateNew;
        event Action HandleOpen;
        event Action HandleSave;
        event Action HandleSaveAs;
        event Action HandleHelp;

        string InfoBarText { set; }
        Color InfoBarColor { set; }
        string SelectedCellText { set; }
        string CellValueBoxText { set; }

        void SetSelection(int col, int row);
        void UpdateCell(int col, int row, string value);
        void DoClose();
        void SetTitle(string spreadsheetName);
    }
}