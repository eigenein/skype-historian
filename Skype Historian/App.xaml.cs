using System;
using System.Reflection;
using System.Resources;
using System.Threading;
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

        private static readonly ResourceManager ResourceManager =
            new ResourceManager(typeof(Properties.Resources));

        private static Mutex mutex = new Mutex(true, "DCFF433C-AA29-4ED0-9F12-B9C143800299");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                Logger.Info("Skype Historian {0} has started.", Version);
            }
            else
            {
                mutex = null;
                Logger.Warn("Application is already running.");
                Xceed.Wpf.Toolkit.MessageBox.Show(ResourceManager.GetString(
                    "AppAlreadyRunning"), "Skype Historian", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Logger.Warn("Quitting.");
                Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
            }
        }

        private void AppDispatcherUnhandledException(object sender, 
            DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.ErrorException("An unhandled exception was raised." +
                Environment.NewLine, e.Exception);
            e.Handled = true;
        }
    }
}
