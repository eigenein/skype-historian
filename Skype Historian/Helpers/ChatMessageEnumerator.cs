using System.Collections;
using System.Collections.Generic;
using Skype4COMWrapper;

namespace SkypeHistorian.Helpers
{
    /// <summary>
    /// Enumerates chat messages in order from the oldest to the newest.
    /// </summary>
    internal class ChatMessageEnumerator : IEnumerator<IChatMessage>
    {
        private readonly IChatMessageCollection messageCollection;

        private readonly int count;

        private int currentIndex;

        private IChatMessage current;

        public static ChatMessageEnumerator Create(IChatMessageCollection messageCollection)
        {
            return new ChatMessageEnumerator(messageCollection);
        }

        private ChatMessageEnumerator(IChatMessageCollection messageCollection)
        {
            this.messageCollection = messageCollection;
            this.count = messageCollection.Count;
            // Message collection indices start from 1.
            // Move to the last (the oldest) message.
            this.currentIndex = this.count;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public bool MoveNext()
        {
            // Message collection indices start from 1.
            if (currentIndex < 1)
            {
                return false;
            }

            current = messageCollection[currentIndex];
            // Move to the newer message.
            currentIndex -= 1;
            return true;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public IChatMessage Current
        {
            get
            {
                return current;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
