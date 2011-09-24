using System;
using System.Text.RegularExpressions;
using Skype4COMWrapper;

namespace SkypeHistorian.Exporting.OutputWriters
{
    internal class CsvOutputWriter : SimpleFilesOutputWriter
    {
        private static readonly Regex FixNewLineRegex =
            new Regex("[(\r\n)(\n\r)]", RegexOptions.Compiled);

        public CsvOutputWriter(Storage storage) 
            : base(storage)
        {
            // Do nothing.
        }

        #region Overrides of SimpleFilesOutputWriter

        protected override string Extension
        {
            get { return ".csv"; }
        }

        protected override string Format
        {
            get { return "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}"; }
        }

        protected override object[] ExtractProperties(IChatMessage message)
        {
            return new object[]
            {
                message.Id,
                message.Timestamp,
                message.FromHandle,
                message.FromDisplayName.Replace(';', ','),
                message.Type,
                message.Status,
                message.LeaveReason,
                FixNewLineRegex.Replace(message.Body, " "),
                message.EditedBy,
                message.EditedTimestamp
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
