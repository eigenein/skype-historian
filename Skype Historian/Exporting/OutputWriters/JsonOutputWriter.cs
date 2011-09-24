using System;
using System.Collections.Generic;
using NLog;
using Newtonsoft.Json;
using Skype4COMWrapper;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.OutputWriters
{
    internal class JsonOutputWriter : OutputWriter
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, JsonTextWriter> writers =
            new Dictionary<string, JsonTextWriter>();

        public JsonOutputWriter(Storage storage) : 
            base(storage)
        {
            // Do nothing.
        }

        #region Overrides of OutputWriter

        public override bool StoreMessage(string path, IChat chat, IChatMessage message)
        {
            JsonTextWriter writer;
            if (!writers.TryGetValue(path, out writer))
            {
                writer = CreateWriter(path, chat);
                if (writer == null)
                {
                    return false;
                }
                writers[path] = writer;
            }
            Action retrieveMessageProperties =
                delegate 
                {
                    writer.WriteStartObject();
                    writer.WriteProperty("Id", message.Id);
                    writer.WriteProperty("Body", message.Body);
                    writer.WriteProperty("TimeStamp", message.Timestamp);
                    writer.WriteProperty("FromDisplayName", message.FromDisplayName);
                    writer.WriteProperty("EditedBy", message.EditedBy);
                    writer.WriteProperty("EditedTimeStamp", message.EditedTimestamp);
                    writer.WriteProperty("FromHandle", message.FromHandle);
                    writer.WriteProperty("LeaveReason", message.LeaveReason);
                    writer.WriteProperty("Type", message.Type);
                    writer.WriteProperty("Status", message.Status);
                    writer.WriteEndObject();
                };
            if (!SafeInvoker.Invoke(retrieveMessageProperties))
            {
                Logger.Error("Cannot retrieve message properties.");
                return false;
            }
            return true;
        }

        private JsonTextWriter CreateWriter(string path, IChat chat)
        {
            JsonTextWriter writer = new JsonTextWriter(
                    storage.GetWriter(path + ".json"))
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            writer.WriteStartObject();
            Action action = 
                delegate
                {
                    writer.WritePropertyName("Chat");
                    writer.WriteStartObject();
                    writer.WriteProperty("Description", chat.Description);
                    writer.WriteProperty("FriendlyName", chat.FriendlyName);
                    writer.WriteProperty("MyRole", chat.MyRole);
                    writer.WriteProperty("MyStatus", chat.MyStatus);
                    writer.WriteProperty("Name", chat.Name);
                    writer.WriteProperty("Topic", chat.Topic);
                    writer.WriteProperty("Type", chat.Type);
                    writer.WritePropertyName("Members");
                    writer.WriteStartArray();
                    foreach (IUser user in chat.Members)
                    {
                        writer.WriteValue(user.Handle);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                };
            if (!SafeInvoker.Invoke(action))
            {
                Logger.Error("Couldn't store chat properties.");
                return null;
            }
            writer.WriteProperty("ExportStartTime", DateTime.Now);
            writer.WriteProperty("SkypeHistorianVersion", App.Version);
            writer.WritePropertyName("Messages");
            writer.WriteStartArray();
            return writer;
        }

        public override void ShowDataToUser()
        {
            storage.ShowDataToUser(".");
        }

        public override void Close()
        {
            foreach (JsonTextWriter writer in writers.Values)
            {
                writer.WriteEndArray();
                writer.WriteProperty("ExportFinishTime", DateTime.Now);
                writer.WriteEndObject();
            }
            writers.Clear();
            base.Close();
        }

        #endregion
    }
}
