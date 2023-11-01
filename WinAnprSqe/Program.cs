using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using NLog;

namespace WinAnprSqe
{
    static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Initialize NLog (usually not necessary when using nlog.config)
            string path;
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
#if DEBUG
            path = Path.GetDirectoryName(Path.GetDirectoryName(path));
#endif
            
            LogManager.LoadConfiguration($"{path}\\nlog.config");
            
            // Set up global exception handling
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            Application.Run(new MainForm());
        }
        
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error("Произошло необработанное исключение.", e.Exception);
            MessageBox.Show("Произошло необработанное исключение.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.Error("Произошло необработанное исключение.", ex);
                MessageBox.Show("Произошло необработанное исключение.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}