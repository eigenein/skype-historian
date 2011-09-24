using System;
using NLog;
using SkypeHistorian.Exporting;

namespace SkypeHistorian.Controls.Pages
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    internal partial class WelcomePage : AbstractPage
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public WelcomePage(Context context) : base(context)
        {
            InitializeComponent();
        }

        public override void InitializeLocalization()
        {
            welcomeTextLabel.Text = Context.ResourceManager.GetString(
                "WelcomePageWelcomeLabel");
            skypeInstalled.Text = Context.ResourceManager.GetString(
                "WelcomePageSkypeInstalled");
            noteTextLabel.Text = Context.ResourceManager.GetString(
                "WelcomePageNote");
        }

        #region Overrides of AbstractPage

        public override string Id
        {
            get { return "Welcome"; }
        }

        public override string Title
        {
            get { return Context.ResourceManager.GetString("WelcomePageTitle"); }
        }

        public override string Category
        {
            get { return "Welcome"; }
        }

        public override string NextPageId
        {
            get
            {
                return "CheckingForSkype";
            }
        }

        public override ButtonType NextButton
        {
            get
            {
                return ButtonType.Next;
            }
        }

        public override void Initialize()
        {
            Logger.Info("{0} is initializing", Id);
        }

        #endregion
    }
}
