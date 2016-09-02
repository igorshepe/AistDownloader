using System;
using System.Windows.Forms;

namespace FinamDownloader
{
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
         
        static void Main(string[] arg)
        {
            var Args = arg;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main(Args));

             
        }
    }
}
