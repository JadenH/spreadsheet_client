using System;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class LauncherWindow : Form, ILauncherView
    {
        public event Action<string, string> OpenSpreadsheet;

        public LauncherWindow()
        {
            InitializeComponent();
            OpenButton.Click += OpenButtonOnClick;
        }

        private void OpenButtonOnClick(object sender, EventArgs eventArgs)
        {
            OpenSpreadsheet?.Invoke(IPAddressTextBox.Text, SpreadsheetTextBox.Text);
            Close();
        }
    }
}
