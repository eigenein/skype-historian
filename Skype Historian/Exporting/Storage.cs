using System;
using System.IO;
using SkypeHistorian.Events;

namespace SkypeHistorian.Exporting
{
    public abstract class Storage
    {
        protected readonly string Path;

        protected Storage(string path)
        {
            this.Path = path;
        }

        public virtual Stream GetStream(string path)
        {
            return GetWriter(path).BaseStream;
        }

        public event EventHandler<ProgressChangedEventArgs> CloseProgressChanged;

        protected void OnCloseProgressChanged(int value, int maximum)
        {
            EventHandler<ProgressChangedEventArgs> handler = CloseProgressChanged;
            if (handler != null)
            {
                handler(this, new ProgressChangedEventArgs(value, maximum));
            }
        }

        public StreamWriter GetWriter(string path)
        {
            bool createdNew;
            return GetWriter(path, out createdNew);
        }

        public abstract StreamWriter GetWriter(string path, 
            out bool createdNew);

        public abstract void Close();

        public abstract void ShowDataToUser(string path);
    }
}
