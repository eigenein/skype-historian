using System;
using NLog;
using Skype4COMWrapper;

namespace SkypeHistorian.Exporting.OutputWriters
{
    internal class TextFilesOutputWriter : SimpleFilesOutputWriter
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public TextFilesOutputWriter(Storage storage)
            : base(storage)
        {
            // Do nothing.
        }

        #region Overrides of SimpleFilesOutputWriter

        protected override string Extension
        {
            get { return ".txt"; }
        }

        protected override string Format
        {
            get { return "[{0:G}] {1}: {2}"; }
        }

        protected override object[] ExtractProperties(IChatMessage message)
        {
            string body = message.Body;
            if (String.IsNullOrEmpty(body))
            {
                Logger.Info("Message #{0}: empty body - skipped.", message.Id);
                return null;
            }
            return new object[]
            {
                message.Timestamp,
                message.FromDisplayName,
                body
            };
        }

        #endregion

        #region Overrides of OutputWriter

        public override void ShowDataToUser()
        {
            storage.ShowDataToUser(".");
        }

        #endregion
    }
}
