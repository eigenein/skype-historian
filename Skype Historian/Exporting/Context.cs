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

        public int MessagesCount { get; set; }

        public int ExportedChatCount { get; set; }

        public int TotalMessagesCount { get; set; }

        public int ProcessedMessagesCount { get; set; }

        public int DateFilterSkippedCount { get; set; }

        public OutputWriter OutputWriter { get; set; }

        public IGroupingStrategy GroupingStrategy { get; set; }

        public bool IsExportingInProgress { get; set; }

        /// <summary>
        /// Whether to use nicknames instead of fullnames.
        /// </summary>
        public bool UseNicknames { get; set; }

        /// <summary>
        /// Whether to export only messages within the specified dates.
        /// </summary>
        public bool UseDateFilter { get; set; }

        public DateTime DateFilterStartDate { get; set; }

        public DateTime DateFilterEndDate { get; set; }

        #endregion
    }
}
