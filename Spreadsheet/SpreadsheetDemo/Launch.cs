using System;
using System.Windows.Forms;


namespace SSGui
{
    /// <summary>
    /// Runs a demo involving a SpreadsheetPanel
    /// </summary>
    static class Launch
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SpreadsheetDemo());
        }
    }
}
