using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SQLDepLib;

namespace SQLDep
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Log(String.Format("\n\n\nStarting program."));
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                string message = String.Format("Some wild exception occured: \n----\n{0} \n----\n", e.Message);
                Logger.Exception(message);
                message = "";
                if (e.InnerException != null)
                {
                    message += e.InnerException.Message;
                }

                Logger.Exception(message);
            }
            Logger.Log("\nExiting program.\n\n");
        }
    }
}
