using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Windows.Controls;
using NLog;
using SkypeHistorian.Exporting;
using MessageBox = Microsoft.Windows.Controls.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;

namespace SkypeHistorian.Controls.Pages
{
    /// <summary>
    /// Interaction logic for OutputTypePage.xaml
    /// </summary>
    public partial class OutputTypePage : AbstractPage
    {
        private readonly Dictionary<RadioButton, OutputType> controlToOutputTypeCache;

        private static readonly GroupingStrategyType[] IndexToStrategyCache =
            new GroupingStrategyType[]
            {
                GroupingStrategyType.ByMembers,
                GroupingStrategyType.ByMonthThenByMembers,
                GroupingStrategyType.ByMembersThenByMonth,
                GroupingStrategyType.ByMonthThenByDayThenThenByMembers
            };

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                new Dictionary<RadioButton, OutputType>
                {
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
            useNicknamesCheckBox.Content = Context.ResourceManager.GetString(
                "OutputTypePageUseNicknames");
            dateFilterCheckBox.Content = Context.ResourceManager.GetString(
                "OutputTypePageDateFilterTitle");
            andLabel.Content = Context.ResourceManager.GetString(
                "OutputTypePageAnd");
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

            dateFilter1.SelectedDate = DateTime.Now.AddMonths(-1);
            dateFilter2.SelectedDate = DateTime.Now;
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
            Context.UseNicknames = useNicknamesCheckBox.IsChecked == true;
            Context.UseDateFilter = dateFilterCheckBox.IsChecked == true;
            if (Context.UseDateFilter)
            {
                if (!SetDateFilter())
                {
                    Logger.Info("Invalid date filter: filter1 is {0}, filter2 is {1}",
                        dateFilter1.SelectedDate, dateFilter2.SelectedDate);
                    return false;
                }
            }
            return true;
        }

        private IGroupingStrategy CreateGroupingStrategy()
        {
            return GroupingStrategyFactory.Create(
                IndexToStrategyCache[groupComboBox.SelectedIndex]);
        }

        private bool SetDateFilter()
        {
            if (!CheckDateFilter())
            {
                return false;
            }
            DateTime date1 = (DateTime)dateFilter1.SelectedDate;
            DateTime date2 = (DateTime)dateFilter2.SelectedDate;
            if (date1 <= date2)
            {
                Context.DateFilterStartDate = date1.Date;
                Context.DateFilterEndDate = date2.Date.AddDays(1);
            }
            else
            {
                Context.DateFilterStartDate = date2.Date;
                Context.DateFilterEndDate = date1.Date.AddDays(1);
            }
            Logger.Info("Using date filter: from {0} to {1}",
                Context.DateFilterStartDate, Context.DateFilterEndDate);
            return true;
        }

        private bool CheckDateFilter()
        {
            DatePicker[] controls = new[] { dateFilter1, dateFilter2 };
            foreach (DatePicker control in controls.Where(control => control.SelectedDate == null))
            {
                control.Focus();
                MessageBox.Show(Context.WindowOwner, Context.ResourceManager.GetString(
                    "OutputTypePageDateFilterError"), "Skype Historian",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private static string GetDefaultPathName()
        {
            return String.Format("Skype {0:MMM-d-yyyy-HH-mm-ss}",
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

        private void DateFilterCheckBoxChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            dateFilter1.IsEnabled = dateFilter2.IsEnabled = dateFilterCheckBox.IsChecked == true;
        }
    }
}
