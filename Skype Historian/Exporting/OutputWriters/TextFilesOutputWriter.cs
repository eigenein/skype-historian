using System;
using Skype4COMWrapper;

namespace SkypeHistorian.Exporting.OutputWriters
{
    internal class TextFilesOutputWriter : SimpleFilesOutputWriter
    {
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
            return new object[]
            {
                message.Timestamp,
                message.FromDisplayName,
                message.Body
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
