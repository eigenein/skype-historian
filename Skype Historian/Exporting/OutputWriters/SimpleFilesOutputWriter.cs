using System;
using System.Collections.Generic;
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

        private readonly IList<object[]> messages =
            new List<object[]>();

        protected SimpleFilesOutputWriter(Storage storage) 
            : base(storage)
        {
            // Do nothing.
        }

        protected abstract string Extension { get; }

        protected abstract string Format { get; }

        protected abstract object[] ExtractProperties(IChatMessage message);

        public override bool StoreMessage(string path, IChat chat, IChatMessage message)
        {
            object[] properties;
            if (!SafeInvoker.Invoke(() => ExtractProperties(message), out properties))
            {
                Logger.Error("Cannot retrieve the message properties.");
                return false;
            }
            StreamWriter writer = storage.GetWriter(path + Extension);
            writer.WriteLine(Format, properties);
            return true;
        }
    }
}
