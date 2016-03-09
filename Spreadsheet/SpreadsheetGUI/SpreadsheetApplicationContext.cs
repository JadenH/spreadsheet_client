using System.IO;
using System.Windows.Forms;
using SS;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Keeps track of how many top-level forms are running, shuts down
    /// the application when there are no more.
    /// </summary>
    public class SpreadsheetApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int windowCount = 0;
        public ISpreadsheetView window;

        // Singleton ApplicationContext
        private static SpreadsheetApplicationContext context;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static SpreadsheetApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new SpreadsheetApplicationContext();
            }
            return context;
        }

        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        /// <param name="textReader"></param>
        public void RunNew()
        {
            // Create the window and the controller
            if (window == null) window = new SpreadsheetWindow();
            new Controller(window);

            // One more form is running
            windowCount++;

            if (window is SpreadsheetWindow)
            {
                SpreadsheetWindow spreadsheetWindow = window as SpreadsheetWindow;
                // When this form closes, we want to find out
                spreadsheetWindow.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

                // Run the form
                spreadsheetWindow.Show();
            }
        }

        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        /// <param name="textReader"></param>
        public void RunNew(string filePath)
        {
            // Create the window and the controller

            if (window == null) window = new SpreadsheetWindow();
            Controller spreadsheetController = new Controller(window);
            TextReader textReader = new StreamReader(filePath);
            spreadsheetController.ChangeSpreadsheet(new Spreadsheet(textReader));
            spreadsheetController.SetSavedPath(filePath);
            textReader.Close();

            // One more form is running
            windowCount++;

            if (window is SpreadsheetWindow)
            {
                SpreadsheetWindow spreadsheetWindow = window as SpreadsheetWindow;
                // When this form closes, we want to find out
                spreadsheetWindow.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

                // Run the form
                spreadsheetWindow.Show();
            }
        }
    }
}