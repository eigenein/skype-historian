using System;
using System.Linq;
using System.Net;
using System.Text;
using NLog;

namespace SkypeHistorian.Helpers
{
    internal static class UsageStatisticsHelper
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public static void Send()
        {
            Logger.Info("Sending usage statistics ...");
            string url = BuildUrl();
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = String.Format("Skype Historian/{0}", App.Version);
            try
            {
                Logger.Info("GA url: {0}", url);
                byte[] response = client.DownloadData(url);
                Logger.Info("GA response {0}", response.Take(6).SequenceEqual(
                    Encoding.ASCII.GetBytes("GIF89a")) ? "Ok" : "failed");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message);
            }
            Properties.Settings.Default.GaLastVisit = DateTime.Now;
            Properties.Settings.Default.VisitCount += 1;
            Properties.Settings.Default.Save();
        }

        private static string BuildUrl()
        {
            const string utmac = "UA-28081528-1";
            const string utmhn = "eigenein.github.com";
            int utmn = RandomExtensions.Instance.Next(1000000000, Int32.MaxValue);
            int random = RandomExtensions.Instance.Next(1000000000, Int32.MaxValue);
            long today = DateTime.Now.GetUnixTime();
            long firstVisit = Properties.Settings.Default.GaFirstVisit.GetUnixTime();
            long lastVisit = Properties.Settings.Default.GaLastVisit.GetUnixTime();
            int visitCount = Properties.Settings.Default.VisitCount;
            const string referer = "skype-historian/app/referer";
            const string uservar = "-";
            const string utmp = "skype-historian/app/usage";

            StringBuilder urlBuilder = new StringBuilder("http://www.google-analytics.com/__utm.gif?utmwv=1");
            urlBuilder.AppendFormat("&utmn={0}", utmn);
            urlBuilder.Append("&utmsr=-&utmsc=-&utmul=-&utmje=0&utmfl=-&utmdt=-");
            urlBuilder.AppendFormat("&utmhn={0}", utmhn);
            urlBuilder.AppendFormat("&utmr={0}", referer);
            urlBuilder.AppendFormat("&utmp={0}", utmp);
            urlBuilder.AppendFormat("&utmac={0}", utmac);
            urlBuilder.AppendFormat(
                "&utmcc=__utma%3D{0}.{1}.{4}.{5}.{2}.{6}%3B%2B__utmb%3D{0}%3B%2B__utmc%3D{0}%3B%2B__utmz%3D{0}.{2}.2.2.utmccn%3D(direct)%7Cutmcsr%3D(direct)%7Cutmcmd%3D(none)%3B%2B__utmv%3D{2}.{3}%3B",
                GaCookie, random, today, uservar, firstVisit, lastVisit, visitCount);

            return urlBuilder.ToString();
        }

        private static int GaCookie
        {
            get 
            {
                int cookie = Properties.Settings.Default.GaCookie;
                if (cookie == 0)
                {
                    cookie = RandomExtensions.Instance.Next(10000000, 99999999);
                    Properties.Settings.Default.GaCookie = cookie;
                    Properties.Settings.Default.Save();
                }
                Logger.Info("GaCookie: {0}", cookie);
                return cookie;
            }
        }
    }
}
