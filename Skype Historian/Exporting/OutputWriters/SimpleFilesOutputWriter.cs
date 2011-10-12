using System;
using System.IO;
using NLog;
using Skype4COMWrapper;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.OutputWriters
{
    internal abstract class SimpleFilesOutputWriter : OutputWriter
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        protected SimpleFilesOutputWriter(Storage storage) 
            : base(storage)
        {
            // Do nothing.
        }

        protected abstract string Extension { get; }

        protected abstract string Format { get; }

        /// <summary>
        /// Gets a property set or <c>null</c>c> if the message should be skipped.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected abstract object[] ExtractProperties(IChatMessage message);

        public override bool StoreMessage(string path, IChat chat, IChatMessage message)
        {
            object[] properties;
            if (!SafeInvoker.Invoke(() => ExtractProperties(message), out properties))
            {
                Logger.Error("Cannot retrieve the message properties.");
                return false;
            }
            if (properties == null)
            {
                // The message is correctly processed, but should be skipped.
                return true;
            }
            StreamWriter writer = storage.GetWriter(path + Extension);
            writer.WriteLine(Format, properties);
            return true;
        }
    }
}
