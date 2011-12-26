using System;
using System.Collections.Generic;
using System.Windows;
using NLog;
using SkypeHistorian.Exporting;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Controls.Pages
{
    /// <summary>
    /// Interaction logic for FinishingPage.xaml
    /// </summary>
    public partial class FinishingPage : AbstractPage
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<StatusCode, string> StatusCodeToNoteCache =
            new Dictionary<StatusCode, string>()
            {
                {StatusCode.SkypeInitializationFailed, "StatusSkypeInitializationFailed"},
                {StatusCode.SkypeStartFailed, "StatusSkypeStartFailed"},
                {StatusCode.Finished, "StatusFinished"},
                {StatusCode.SkypeCannotGetChatCollection, "StatusSkypeCannotGetChatCollection"}
            };

        private string chatsExportedString;
        private string messagesProcessedString;

        public FinishingPage()
            : this(null)
        {
            // Do nothing.
        }

        public FinishingPage(Context context)
            : base(context)
        {
            InitializeComponent();
        }

        public override string Id
        {
            get
            {
                return "Finishing";
            }
        }

        public override string Title
        {
            get
            {
                return Context.ResourceManager.GetString(
                    "FinishingPageTitle");
            }
        }

        public override ButtonType NextButton
        {
            get
            {
                return ButtonType.Finish;
            }
        }

        public override void Initialize()
        {
            Logger.Info("{0} is initializing", Id);
            string note;
            if (!StatusCodeToNoteCache.TryGetValue(Context.StatusCode, out note))
            {
                note = "StatusUnknownFailure";
            }
            noteLabel.Text = String.Format(
                Context.ResourceManager.GetString(note),
                Context.StatusCode);
            chatsExportedLabel.Content = String.Format(
                chatsExportedString, Context.ExportedChatCount, Context.ChatCount);
            messagesExportedLabel.Content = String.Format(
                messagesProcessedString, Context.ProcessedMessagesCount);
            showExportedCheckBox.IsChecked = Context.StatusCode == StatusCode.Finished;
            showExportedCheckBox.IsEnabled = Context.ProcessedMessagesCount > 0;
            startAgainCheckBox.IsChecked = Context.StatusCode != StatusCode.Finished;

            NativeMethods.MakeTheTaskbarBlinkMyApplication();
        }

        public override void InitializeLocalization()
        {
            chatsExportedString = Context.ResourceManager.GetString(
                "ChatsExported");
            messagesProcessedString = Context.ResourceManager.GetString(
                "MessagesProcessed");
            showExportedCheckBox.Content = Context.ResourceManager.GetString(
                "FinishingPageShowExported");
            startAgainCheckBox.Content = Context.ResourceManager.GetString(
                "FinishingPageStartAgain");
        }

        public override void OnFinish()
        {
            if (showExportedCheckBox.IsChecked == true)
            {
                Logger.Info("Showing exported data ...");
                Context.OutputWriter.ShowDataToUser();
            }
            if (startAgainCheckBox.IsChecked == true)
            {
                Logger.Info("Starting the new process ...");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            }
            Logger.Info("Quit.");
            Application.Current.Shutdown();
        }
    }
}
