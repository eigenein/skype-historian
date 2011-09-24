using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NLog;

namespace SkypeHistorian.Exporting.Storages
{
    internal class ZipFileStorage : Storage
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, StreamWriter> pathToWriterCache =
            new Dictionary<string, StreamWriter>();

        public ZipFileStorage(string path) 
            : base(path)
        {
            // Do nothing.
        }

        #region Overrides of Storage

        public override StreamWriter GetWriter(string path,
            out bool createdNew)
        {
            StreamWriter writer;
            createdNew = !pathToWriterCache.TryGetValue(path, out writer);
            if (createdNew)
            {
                writer = pathToWriterCache[path] =
                    new StreamWriter(new MemoryStream(), Encoding.UTF8);
            }
            return writer;
        }

        public override void Close()
        {
            Logger.Info("Flushing writers ...");
            foreach (StreamWriter writer in pathToWriterCache.Values)
            {
                writer.Flush();
            }
            Logger.Info("Compressing ...");
            OnCloseProgressChanged(0, pathToWriterCache.Count);
            using (ZipOutputStream zipStream = new ZipOutputStream(
                File.OpenWrite(Path)))
            {
                zipStream.SetLevel(9);
                zipStream.UseZip64 = UseZip64.Off;
                byte[] buffer = new byte[4096];
                int closedCount = 0;
                foreach (KeyValuePair<string, StreamWriter> pathWriterPair
                    in pathToWriterCache)
                {
                    Stream inputStream = pathWriterPair.Value.BaseStream;
                    inputStream.Seek(0, SeekOrigin.Begin);
                    ZipEntry entry = new ZipEntry(
                        ZipEntry.CleanName(pathWriterPair.Key));
                    entry.DateTime = DateTime.Now;
                    zipStream.PutNextEntry(entry);
                    StreamUtils.Copy(inputStream, zipStream, buffer);
                    zipStream.CloseEntry();
                    OnCloseProgressChanged(closedCount++, pathToWriterCache.Count);
                }
                zipStream.Close();
            }
            pathToWriterCache.Clear();
            Logger.Info("Closed.");
        }

        public override void ShowDataToUser(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe",
                "/select, " + Path);
        }

        #endregion
    }
}
