using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SSGui;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    { 
        public event Func<bool> HandleClose;
        public event Action<string> CellValueBoxTextChange;
        public event Action<int, int> CellSelectionChange;
        public event Action CreateNew;
        public event Action HandleOpen;
        public event Action HandleSave;
        public event Action HandleSaveAs;
        public event Action HandleHelp;

        public SpreadsheetWindow()
        {
            InitializeComponent();
            CellValueBox.TextChanged += CellValueBoxOnTextChanged;
            SpreadsheetData.SelectionChanged += SpreadsheetDataOnSelectionChanged;
        }

        protected void CellValueBoxOnTextChanged(dynamic sender, EventArgs eventArgs)
        {
            CellValueBoxTextChange?.Invoke(sender.Text);
        }

        public string InfoBarText
        {
            set { DataBar.Text = value; }
        }

        public Color InfoBarColor
        {
            set { DataBar.ForeColor = value; }
        }

        public string SelectedCellText
        {
            set { SelectedCellName.Text = value; }
        }

        public string CellValueBoxText
        {
            set { CellValueBox.Text = value; }
        }

        public void SetSelection(int col, int row)
        {
            SpreadsheetData.SetSelection(col, row);
        }

        public void UpdateCell(int col, int row, string value)
        {
            SpreadsheetData.SetValue(col, row, value);
        }

        public void DoClose()
        {
            Close();
        }

        public void SetTitle(string spreadsheetName)
        {
            Text = spreadsheetName;
        }

        private void SpreadsheetDataOnSelectionChanged(SpreadsheetPanel sender)
        {
            int col, row;
            sender.GetSelection(out col, out row);
            CellSelectionChange?.Invoke(col, row);
        }

        private void SpreadsheetWindow_OnClose(object sender, CancelEventArgs e)
        {
            //Confirm closing if there is potential data loss.
            if (HandleClose != null) e.Cancel = HandleClose.Invoke();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNew?.Invoke();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleOpen?.Invoke();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleSaveAs?.Invoke();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleSave?.Invoke();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleHelp?.Invoke();
        }
    }
}
