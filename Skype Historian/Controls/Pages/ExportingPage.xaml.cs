using System;
using System.Collections;
using System.Text;
using System.Threading;
using NLog;
using Skype4COMWrapper;
using SkypeHistorian.Events;
using SkypeHistorian.Exporting;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Controls.Pages
{
    /// <summary>
    /// Interaction logic for ExportingPage.xaml
    /// </summary>
    public partial class ExportingPage : AbstractPage
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private readonly Action exportAction;

        private Timer statisticsTimer;

        private string chatsExportedString;
        private string messagesProcessedString;
        private string exportingPageClosingString;
        private string waitingForSkypeString;

        public ExportingPage()
            : this(null)
        {
            // Do nothing.
        }

        public ExportingPage(Context context)
            : base(context)
        {
            InitializeComponent();

            this.exportAction = ExportChats;
        }

        public override void InitializeLocalization()
        {
            note1Label.Text = Context.ResourceManager.GetString(
                "ExportingPageNote1");
            note2Label.Text = Context.ResourceManager.GetString(
                "ExportingPageNote2");
            chatsExportedString = Context.ResourceManager.GetString(
                "ChatsExported");
            messagesProcessedString = Context.ResourceManager.GetString(
                "ExportingPageMessagesProcessed");
            exportingPageClosingString = Context.ResourceManager.GetString(
                "ExportingPageClosing");
            waitingForSkypeString = Context.ResourceManager.GetString(
                "WaitingForSkypeString");
        }

        public override string Title
        {
            get
            {
                return Context.ResourceManager.GetString(
                    "ExportingPageTitle");
            }
        }

        public override string Id
        {
            get
            {
                return "Exporting";
            }
        }

        public override void Initialize()
        {
            Logger.Info("{0} is initializing", Id);
            exportAction.BeginInvoke(ExportCallback, null);
            statisticsTimer = new Timer(UpdateStatistics, null, 100, 100);
        }

        private void ExportCallback(IAsyncResult ar)
        {
            exportAction.EndInvoke(ar);
            statisticsTimer.Dispose();
            statisticsTimer = null;
            Dispatcher.Invoke(new Action(() => OnPageChangeRequested("Finishing")));
        }

        private void ExportChats()
        {
            Context.IsExportingInProgress = true;
            Logger.Info("About to get chats collection ...");
            IEnumerator chatEnumerator;
            if (!GetChatCollection(out chatEnumerator))
            {
                Context.StatusCode = StatusCode.SkypeCannotGetChatCollection;
            }
            else if (Context.ChatCount > 0)
            {
                Context.ProgressStartedAt = DateTime.Now;
                Dispatcher.Invoke(new Action(
                    () => bottomInfoLabel.Content = String.Format(
                        messagesProcessedString, 0, "-", "--")));
                Dispatcher.Invoke(new Action(
                    () => progressBar.Maximum = Context.MessagesCount));
                while (true)
                {
                    IChat chat;
                    if (!SafeInvoker.Invoke(() =>
                        chatEnumerator.MoveNext() ? (IChat)chatEnumerator.Current : null,
                        out chat))
                    {
                        Context.StatusCode = StatusCode.SkypeCannotGetChat;
                        break;
                    }
                    if (chat == null)
                    {
                        Logger.Info("Chat collection processing is finished.");
                        break;
                    }
                    if (ExportChat(chat))
                    {
                        Context.ExportedChatCount += 1;
                        Dispatcher.Invoke(new Action(
                            () => topInfoLabel.Content = String.Format(
                                chatsExportedString,
                                Context.ExportedChatCount, Context.ChatCount)));
                    }
                    else
                    {
                        Logger.Warn(
                            "The chat is skipped to the error: {0}",
                            Context.StatusCode);
                        // Reset the error as we've just skipped this error.
                        Context.StatusCode = StatusCode.NoError;
                    }
                    Logger.Info("Date filter skipped count: {0}", Context.DateFilterSkippedCount);
                }
            }

            Dispatcher.Invoke(new Action(
                () => topInfoLabel.Content = String.Empty));
            Dispatcher.Invoke(new Action(
                () => bottomInfoLabel.Content = String.Empty));
            Context.OutputWriter.Storage.CloseProgressChanged += StorageCloseProgressChanged;
            Context.OutputWriter.Close();
            Logger.Info("{0} messages of {1} are exported.",
                Context.ProcessedMessagesCount, Context.MessagesCount);
            Logger.Info("{0} chats of {1} are exported.",
                Context.ExportedChatCount, Context.ChatCount);
            if (Context.StatusCode == StatusCode.NoError)
            {
                Context.StatusCode = StatusCode.Finished;
            }
            Context.IsExportingInProgress = false;
        }

        private void StorageCloseProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(
                () => bottomInfoLabel.Content = String.Format(
                    exportingPageClosingString, e.Value, e.Maximum)));
        }

        private bool GetChatCollection(out IEnumerator enumerator)
        {
            enumerator = null;

            IChatCollection chatCollection;
            Dispatcher.Invoke(new Action(
                () => bottomInfoLabel.Content = waitingForSkypeString));
            Thread.Sleep(100); // I don't understand why, but it helps to "unhang" showing this page.
            Logger.Info("Awaiting for Skype response ...");
            if (!SafeInvoker.Repeat(() => Context.Skype.Chats,
                out chatCollection, 3))
            {
                return false;
            }
            int chatCount;
            if (!SafeInvoker.Invoke(() => chatCollection.Count, out chatCount))
            {
                return false;
            }
            Context.ChatCount = chatCount;
            int messagesCount;
            // ReSharper disable UseIndexedProperty
            // Because indexed property fail an installer building.
            if (!SafeInvoker.Invoke(() => Context.Skype.get_Messages(String.Empty).Count, 
            // ReSharper restore UseIndexedProperty
                out messagesCount))
            {
                return false;
            }
            Context.MessagesCount = messagesCount;
            Logger.Info("{0} chat(s) and {1} messages found.", chatCount, messagesCount);
            return SafeInvoker.Invoke(chatCollection.GetEnumerator,
                out enumerator);
        }

        private bool ExportChat(IChat chat)
        {
            Logger.Info("Started processing a chat: {0}.", chat.Name);
            IEnumerator messageEnumerator;
            if (!GetMessageCollection(chat, out messageEnumerator))
            {
                Context.StatusCode = StatusCode.CannotGetMessageCollection;
                return false;
            }
            string members;
            if (!SafeInvoker.Invoke(() => GetMembersString(chat), out members))
            {
                Context.StatusCode = StatusCode.SkypeCannotGetChatProperties;
                return false;
            }
            Logger.Info("Started processing {0} chat.", members);
            while (true)
            {
                IChatMessage message;
                if (!SafeInvoker.Invoke(() =>
                    messageEnumerator.MoveNext() ? (IChatMessage)messageEnumerator.Current : null,
                    out message))
                {
                    Context.StatusCode = StatusCode.SkypeCannotGetMessage;
                    break;
                }
                if (message == null)
                {
                    Logger.Info("Messages processing is finished.");
                    break;
                }
                DateTime messageTimeStamp;
                if (!SafeInvoker.Invoke(() => message.Timestamp, out messageTimeStamp))
                {
                    Context.StatusCode = StatusCode.SkypeCannotGetMessageTimeStamp;
                    break;
                }
                if (!Context.UseDateFilter ||
                    (Context.DateFilterStartDate <= messageTimeStamp && Context.DateFilterEndDate > messageTimeStamp))
                {
                    string chatPath = Context.GroupingStrategy.GetChatPathForMessage(
                        members, messageTimeStamp);
                    if (!Context.OutputWriter.StoreMessage(chatPath, chat, message))
                    {
                        Context.StatusCode = StatusCode.MessageStoringFailed;
                        break;
                    }
                }
                else
                {
                    Context.DateFilterSkippedCount += 1;
                }
                Context.ProcessedMessagesCount += 1;
            }
            return true;
        }

        private static string GetMembersString(IChat chat)
        {
            StringBuilder builder = new StringBuilder();
            foreach (IUser user in chat.Members)
            {
                if (builder.Length != 0)
                {
                    builder.Append(", ");
                }
                string fullName = user.FullName;
                builder.Append(!Context.Instance.UseNicknames && !String.IsNullOrEmpty(fullName) ?
                    fullName : user.Handle);
            }
            if (builder.Length == 0)
            {
                builder.Append("(No members)");
            }
            if (builder.Length > 64)
            {
                builder.Remove(64, builder.Length - 64);
                builder.Append("...");
            }
            return builder.ToString();
        }

        private static bool GetMessageCollection(IChat chat, out IEnumerator enumerator)
        {
            enumerator = null;

            IChatMessageCollection messageCollection;
            if (!SafeInvoker.Invoke(() => chat.Messages, out messageCollection))
            {
                return false;
            }
            int messageCount;
            if (!SafeInvoker.Invoke(() => messageCollection.Count, out messageCount))
            {
                return false;
            }
            Logger.Info("{0} message(s) found in this chat.", messageCount);
            return SafeInvoker.Invoke(messageCollection.GetEnumerator,
                out enumerator);
        }

        private void UpdateStatistics(object state)
        {
            if (Context.ProcessedMessagesCount > 0)
            {
                Context.Eta = new TimeSpan((Context.MessagesCount - Context.ProcessedMessagesCount) *
                    (DateTime.Now - Context.ProgressStartedAt).Ticks / Context.ProcessedMessagesCount);
            }
            Dispatcher.Invoke(new Action(() => bottomInfoLabel.Content = String.Format(
                messagesProcessedString, Context.ProcessedMessagesCount,
                Context.Eta.Minutes, Context.Eta.Seconds)));
            Dispatcher.Invoke(new Action(() => progressBar.Value = Context.ProcessedMessagesCount));
        }
    }
}
