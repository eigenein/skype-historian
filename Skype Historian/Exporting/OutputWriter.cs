using System;
using Skype4COMWrapper;

namespace SkypeHistorian.Exporting
{
    public abstract class OutputWriter
    {
        protected readonly Storage storage;

        protected OutputWriter(Storage storage)
        {
            this.storage = storage;
        }

        public abstract bool StoreMessage(string path, IChat chat,
            IChatMessage message);

        public abstract void ShowDataToUser();

        public Storage Storage
        {
            get { return storage; }
        }

        public virtual void Close()
        {
            storage.Close();
        }
    }
}
