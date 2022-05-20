// AUTHOR:   Scott Crowley (u1178178)
// VERSION:  6 October 2019

using System;
using System.Windows.Forms;

namespace SS
{  
    /// <summary>
    /// Tracks how many top-level spreadsheets are running.
    /// </summary>
    class SpreadsheetAppContext : ApplicationContext
    {
        private int _count = 0;     // Number of open spreadsheets.

        // Singleton ApplicationContext
        private static SpreadsheetAppContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        private SpreadsheetAppContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadsheetAppContext.
        /// </summary>
        /// <returns></returns>
        public static SpreadsheetAppContext getAppContext()
        {
            if (appContext is null)
                appContext = new SpreadsheetAppContext();
            return appContext;
        }

        /// <summary>
        /// Runs the spreadsheet.
        /// </summary>
        public void RunSpreadsheet(Form ss)
        {
            _count++;

            // Listen for spreadsheet closure and decrement count. Exit thread if it was the last one.
            ss.FormClosed += (o, e) => { if (--_count <= 0) ExitThread(); };

            ss.Show();
        }
    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one spreadsheet inside it.
            SpreadsheetAppContext appContext = SpreadsheetAppContext.getAppContext();
            appContext.RunSpreadsheet(new SpreadsheetGUI());
            Application.Run(appContext);
        }
    }
}
