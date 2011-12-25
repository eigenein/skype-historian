using System;
using System.Net;
using System.Text;
using NLog;

namespace SkypeHistorian.Helpers
{
    /// <summary>
    /// Used to check for updates.
    /// </summary>
    internal static class UpdatesChecker
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private static readonly Uri LatestVersionUri =
            new Uri("https://raw.github.com/eigenein/skype-historian/gh-pages/latest-version.txt");

        public static bool CheckUpdateAvailable()
        {
            try
            {
                Logger.Info("Checking for updates ...");
                WebClient client = new WebClient();
                client.Encoding = Encoding.ASCII;
                string versionString = client.DownloadString(LatestVersionUri);
                Logger.Info("Latest version is {0}", versionString);
                Version latestVersion = new Version(versionString);
                return latestVersion > App.Version;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message);
                return false;
            }
        }
    }
}
