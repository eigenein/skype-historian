using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using SkypeHistorian.Exporting;
using RadioButton = System.Windows.Controls.RadioButton;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace SkypeHistorian.Controls.Pages
{
    /// <summary>
    /// Interaction logic for OutputTypePage.xaml
    /// </summary>
    public partial class OutputTypePage : AbstractPage
    {
        private readonly Dictionary<RadioButton, OutputType>
            controlToOutputTypeCache;

        private static readonly GroupingStrategyType[] IndexToStrategyCache =
            new GroupingStrategyType[]
            {
                GroupingStrategyType.ByMembers,
                GroupingStrategyType.ByMonthThenByMembers,
                GroupingStrategyType.ByMembersThenByMonth,
                GroupingStrategyType.ByMonthThenByDayThenThenByMembers
            };

        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public OutputTypePage()
            : this(null)
        {
            // Do nothing.
        }

        public OutputTypePage(Context context)
            : base(context)
        {
            InitializeComponent();

            controlToOutputTypeCache =
                new Dictionary<RadioButton, OutputType>()
                {
                    {htmlRadioButton, OutputType.Html},
                    {jsonRadioButton, OutputType.Json},
                    {xmlRadioButton, OutputType.Xml},
                    {textFilesRadioButton, OutputType.TextFiles},
                    {csvRadioButton, OutputType.Csv}
                };
        }

        public override void InitializeLocalization()
        {
            noteLabel.Text = Context.ResourceManager.GetString(
                "OutputTypePageNote");
            textFilesLabel.Text = Context.ResourceManager.GetString(
                "OutputTypePageTextFiles");
            compressCheckBox.Content = Context.ResourceManager.GetString(
                "OutputTypePageCompress");
            chooseThisNote.Text = Context.ResourceManager.GetString(
                "OutputTypePageChooseThis");
            groupMessagesLabel.Content = Context.ResourceManager.GetString(
                "OutputTypePageGroup");
            byMembersItem.Content = Context.ResourceManager.GetString(
                "OutputTypePageByMembers");
            byMembersThenByMonthItem.Content = Context.ResourceManager.GetString(
                "OutputTypePageByMembersThenByMonth");
            byMonthThenByMembersItem.Content = Context.ResourceManager.GetString(
                "OutputTypePageByMonthThenByMembers");
            byMonthThenByDaysThenByMembersItem.Content = Context.ResourceManager.GetString(
                "OutputTypePageByMonthThenByDayThenByMembers");
        }

        public override string Category
        {
            get { return "Options"; }
        }

        public override string Id
        {
            get { return "OutputType"; }
        }

        public override string Title
        {
            get { return Context.ResourceManager.GetString("OutputTypePageTitle"); }
        }

        public override ButtonType NextButton
        {
            get
            {
                return ButtonType.Next;
            }
        }

        public override string NextPageId
        {
            get
            {
                return "Exporting";
            }
        }

        public override void Initialize()
        {
            Logger.Info("{0} is initializing", Id);
        }

        public override bool OnNext()
        {
            bool compressData = compressCheckBox.IsChecked == true;
            string path;
            if (!AskForFolder(out path))
            {
                return false;
            }
            if (compressData)
            {
                path += ".zip";
            }
            Storage storage = StorageFactory.Create(path, compressData);
            OutputType outputType = controlToOutputTypeCache.First(
                entry => entry.Key.IsChecked == true).Value;
            Context.OutputWriter = OutputWriterFactory.Create(
                outputType, storage);
            Context.GroupingStrategy = CreateGroupingStrategy();
            return true;
        }

        private IGroupingStrategy CreateGroupingStrategy()
        {
            return GroupingStrategyFactory.Create(
                IndexToStrategyCache[groupComboBox.SelectedIndex]);
        }

        private static string GetDefaultPathName()
        {
            return String.Format("Backup {0:MMMM-d-yyyy-HH-mm-ss}",
                DateTime.Now);
        }

        private bool AskForFolder(out string path)
        {
            path = null;
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Description = Context.ResourceManager.GetString(
                    "OutputTypePageFolderBrowserTitle")
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = Path.Combine(dialog.SelectedPath,
                    GetDefaultPathName());
                return true;
            }
            return false;
        }
    }
}
