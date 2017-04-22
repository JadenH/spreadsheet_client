using System;
using System.IO;
using System.Windows.Forms;
using Network;
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
        public bool IsUnitTest;

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

        public void RunLauncher()
        {
            // Create the window and the controller
            LauncherWindow launcherWindow = new LauncherWindow();

            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            launcherWindow.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };
            if (!IsUnitTest)
            {
                // Run the form
                launcherWindow.Show();
            }

            launcherWindow.OpenSpreadsheet += RunNew;
        }

        public void TestDelegate()
        {
            
        }

        public delegate void Test();
        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        /// <param name="textReader"></param>
        public void RunNew(string ipaddress, string spreadsheetName)
        {
            var server = new ServerConnection();

            // Create the window and the controller
            SpreadsheetWindow spreadsheetWindow = new SpreadsheetWindow();
            var controller = new Controller(spreadsheetWindow, server,spreadsheetName);

            server.ClientDisconnected += () => spreadsheetWindow.Invoke(new Test(() =>
            {
                RunLauncher();
                spreadsheetWindow.Close();
            }));

            server.MessageReceived += s => spreadsheetWindow.Invoke(new Test(() => { controller.MessageReceived(s); }));
            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            spreadsheetWindow.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };
            if (!IsUnitTest)
            {
                // Run the form
                spreadsheetWindow.Show();
            }

            server.Connect(ipaddress, spreadsheetName);
        }

    }
}