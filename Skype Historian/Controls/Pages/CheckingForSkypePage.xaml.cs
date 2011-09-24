using System;
using NLog;
using SkypeHistorian.Exporting;
using SkypeHistorian.Helpers;

namespace SkypeHistorian.Controls.Pages
{
    /// <summary>
    /// Interaction logic for CheckingForSkypePage.xaml
    /// </summary>
    public partial class CheckingForSkypePage : AbstractPage
    {
        private readonly Action startSkypeAction;

        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public CheckingForSkypePage()
            : this(null)
        {
            // Do nothing,
        }

        public CheckingForSkypePage(Context context)
            : base(context)
        {
            InitializeComponent();

            startSkypeAction = StartSkype;
        }

        public override string Id
        {
            get
            {
                return "CheckingForSkype";
            }
        }

        public override string Title
        {
            get
            {
                return Context.ResourceManager.GetString("CheckingForSkypePageTitle");
            }
        }

        public override string Category
        {
            get
            {
                return "Skype";
            }
        }

        public override void InitializeLocalization()
        {
            noteLabel.Text = Context.ResourceManager.GetString(
                "CheckingForSkypePageNote");
        }

        public override void Initialize()
        {
            Logger.Info("{0} is initializing", Id);
            startSkypeAction.BeginInvoke(StartSkypeCallback, null);
        }

        private void StartSkypeCallback(IAsyncResult ar)
        {
            startSkypeAction.EndInvoke(ar);
            Dispatcher.Invoke(new Action(() => OnPageChangeRequested(
                Context.StatusCode == StatusCode.NoError ? "OutputType" : "Finishing"))
            );
        }

        private void StartSkype()
        {
            Skype4COMWrapper.Skype skype;
            if (!SafeInvoker.Invoke(() => new Skype4COMWrapper.Skype(), out skype))
            {
                Context.StatusCode = StatusCode.SkypeInitializationFailed;
                return;
            }
            Context.Skype = skype;
            Action action = delegate
                            {
                                if (!skype.Client.IsRunning)
                                {
                                    skype.Client.Start(true, true);
                                }
                            };
            if (!SafeInvoker.Invoke(action))
            {
                Context.StatusCode = StatusCode.SkypeStartFailed;
                return;
            }
        }
    }
}
