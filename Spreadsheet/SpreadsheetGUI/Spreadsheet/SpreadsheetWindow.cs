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
        public event Action<string> CellValueBoxTextComplete;
        public event Action<int, int> CellSelectionChange;
        public event Action CreateNew;
        public event Action HandleOpen;
        public event Action HandleSave;
        public event Action HandleSaveAs;
        public event Action HandleHelp;
        public event Action HandleUndo;

        public SpreadsheetWindow()
        {
            InitializeComponent();
            SpreadsheetData.SelectionChanged += SpreadsheetDataOnSelectionChanged;
            CellValueBox.KeyDown += CellValueBoxOnKeyDown;
            CellValueBox.Leave += CellValueBoxOnLostFocus;
            undoToolStripMenuItem.Click += DoUndo;
        }

        private void DoUndo(object sender, EventArgs eventArgs)
        {
            HandleUndo?.Invoke();
        }

        private void CellValueBoxOnLostFocus(dynamic sender, EventArgs eventArgs)
        {
            CellValueBoxTextComplete?.Invoke(sender.Text);
        }

        private void CellValueBoxOnKeyDown(dynamic sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Enter)
            {
                CellValueBoxTextComplete?.Invoke(sender.Text);
            }
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
            set
            {
                // InvokeRequired required compares the thread ID of the
                // calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (CellValueBox.InvokeRequired)
                {
                    Invoke(new Action<string>(s =>
                    {
                        CellValueBoxText = s;
                    }), value);
                }
                else
                {
                    CellValueBox.Text = value;
                }
            }
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

        public void CellBackgroundColor(int col, int row, Color color)
        {
            SpreadsheetData.SetCellBackground(col, row, color);
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
