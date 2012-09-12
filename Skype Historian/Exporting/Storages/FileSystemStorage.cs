using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NLog;

namespace SkypeHistorian.Exporting.Storages
{
    internal class FileSystemStorage : Storage
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, StreamWriter> writers =
            new Dictionary<string, StreamWriter>();

        public FileSystemStorage(string path) 
            : base(path)
        {
            // Do nothing.
        }

        #region Overrides of Storage

        public override StreamWriter GetWriter(string path,
            out bool createdNew)
        {
            StreamWriter writer;
            createdNew = !writers.TryGetValue(path, out writer);
            if (createdNew)
            {
                string fullPath = System.IO.Path.Combine(Path, path);
                string directoryPath = System.IO.Path.GetDirectoryName(fullPath);
                if (!String.IsNullOrEmpty(directoryPath) && 
                    !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                Logger.Debug("Creating a file: {0}", fullPath);
                writer = writers[path] = new StreamWriter(File.OpenWrite(fullPath),
                    Encoding.UTF8);
            }
            return writer;
        }

        public override void Close()
        {
            Logger.Info("Closing streams ...");
            OnCloseProgressChanged(0, writers.Count);
            int closedCount = 0;
            foreach (StreamWriter writer in writers.Values)
            {
                writer.Close();
                OnCloseProgressChanged(closedCount++, writers.Count);
            }
            writers.Clear();
        }

        public override void ShowDataToUser(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe",
                "/select, " + System.IO.Path.Combine(Path, path));
        }

        #endregion
    }
}
