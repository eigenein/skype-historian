using System;
using System.Resources;
using System.Windows;
using NLog;
using Skype4COMWrapper;

namespace SkypeHistorian.Exporting
{
    public class Context
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        #region Singleton

        private static readonly Context instance = new Context();

        private Context()
        {
            // Do nothing.
        }

        public static Context Instance
        {
            get { return instance; }
        }

        #endregion

        #region Properties

        public ResourceManager ResourceManager { get; set; }

        public Skype Skype { get; set; }

        private StatusCode statusCode;

        public StatusCode StatusCode
        {
            get { return statusCode; }
            set
            {
                statusCode = value;
                Logger.Info("StatusCode: {0}", value);
            }
        }

        public Window WindowOwner { get; set; }

        public int ChatCount { get; set; }

        public int ExportedChatCount { get; set; }

        public int TotalMessagesCount { get; set; }

        public int ExportedMessagesCount { get; set; }

        public OutputWriter OutputWriter { get; set; }

        public IGroupingStrategy GroupingStrategy { get; set; }

        public bool IsExportingInProgress { get; set; }

        #endregion
    }
}
