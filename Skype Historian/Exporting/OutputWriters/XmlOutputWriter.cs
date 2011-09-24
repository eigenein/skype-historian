using System;
using System.Collections.Generic;
using System.Xml;
using NLog;
using Skype4COMWrapper;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Exporting.OutputWriters
{
    internal class XmlOutputWriter : OutputWriter
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, XmlTextWriter> writers =
            new Dictionary<string, XmlTextWriter>();

        public XmlOutputWriter(Storage storage)
            : base(storage)
        {
            // Do nothing.
        }

        #region Overrides of OutputWriter

        public override bool StoreMessage(string path, IChat chat, IChatMessage message)
        {
            XmlTextWriter writer;
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
                    writer.WriteStartElement("Message");
                    writer.WriteAttribute("Id", message.Id);
                    writer.WriteAttribute("TimeStamp", message.Timestamp);
                    writer.WriteAttribute("FromDisplayName", message.FromDisplayName);
                    writer.WriteAttribute("EditedBy", message.EditedBy);
                    writer.WriteAttribute("EditedTimeStamp", message.EditedTimestamp);
                    writer.WriteAttribute("FromHandle", message.FromHandle);
                    writer.WriteAttribute("LeaveReason", message.LeaveReason);
                    writer.WriteAttribute("Type", message.Type);
                    writer.WriteAttribute("Status", message.Status);
                    writer.WriteString(message.Body);
                    writer.WriteEndElement();
                };
            if (!SafeInvoker.Invoke(retrieveMessageProperties))
            {
                Logger.Error("Cannot retrieve message properties.");
                return false;
            }
            return true;
        }

        public override void ShowDataToUser()
        {
            storage.ShowDataToUser(".");
        }

        public override void Close()
        {
            foreach (XmlTextWriter writer in writers.Values)
            {
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            writers.Clear();
            base.Close();
        }

        #endregion

        private XmlTextWriter CreateWriter(string path, IChat chat)
        {
            XmlTextWriter writer = new XmlTextWriter(
                storage.GetWriter(path + ".xml"))
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
            writer.WriteStartDocument();
            writer.WriteStartElement("Chat");
            writer.WriteAttribute("ExportStartTime", DateTime.Now);
            writer.WriteAttribute("SkypeHistorianVersion", App.Version);
            Action action =
                delegate
                {
                    writer.WriteAttribute("Description", chat.Description);
                    writer.WriteAttribute("FriendlyName", chat.FriendlyName);
                    writer.WriteAttribute("MyRole", chat.MyRole);
                    writer.WriteAttribute("MyStatus", chat.MyStatus);
                    writer.WriteAttribute("Name", chat.Name);
                    writer.WriteAttribute("Topic", chat.Topic);
                    writer.WriteAttribute("Type", chat.Type);
                    writer.WriteStartElement("Members");
                    foreach (IUser user in chat.Members)
                    {
                        writer.WriteStartElement("Member");
                        writer.WriteAttribute("Handle", user.Handle);
                        writer.WriteAttribute("FullName", user.FullName);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                };
            if (!SafeInvoker.Invoke(action))
            {
                Logger.Error("Couldn't store chat properties.");
                return null;
            }
            writer.WriteStartElement("Messages");
            return writer;
        }
    }
}
