using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using NLog;

namespace SkypeHistorian
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public static readonly Version Version =
            Assembly.GetExecutingAssembly().GetName().Version;

        public App()
        {
            Logger.Info("Skype Historian {0} has started.", Version);
        }

        void AppDispatcherUnhandledException(object sender, 
            DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.ErrorException("An unhandled exception was raised." +
                Environment.NewLine, e.Exception);
            e.Handled = true;
        }
    }
}
