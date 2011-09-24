using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using SkypeHistorian.Controls.Pages;
using SkypeHistorian.Events;
using SkypeHistorian.Exporting;

namespace SkypeHistorian
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        private static readonly ResourceManager ResourceManager =
            new ResourceManager(typeof(Properties.Resources));

        private static readonly Color SuccessHeadingColor = 
            Color.FromRgb(0xDD, 0xFF, 0xCC);
        private static readonly Color FailureHeadingColor = 
            Color.FromRgb(0xFF, 0xDD, 0xCC);
        private static readonly Color SuccessTitleColor =
            Color.FromRgb(0x00, 0x99, 0x33);
        private static readonly Color FailureTitleColor =
            Color.FromRgb(0x99, 0x00, 0x33);

        private string confirmCloseString;

        private readonly Dictionary<string, Label> categoryToLabelCache;

        public MainWindow()
        {
            Context.Instance.WindowOwner = this;

            InitializeComponent();
            InitializeLocalization();

            categoryToLabelCache = new Dictionary<string, Label>()
            {
                {"Welcome", leftBarWelcomeLabel},
                {"Skype", leftBarSkypeLabel},
                {"Options", leftBarOptionsLabel},
                {"Finishing", leftBarFinishingLabel},
                {"Exporting", leftBarExportingLabel}
            };

            pagesControl.AddPage(new WelcomePage(Context.Instance));
            pagesControl.AddPage(new CheckingForSkypePage(Context.Instance));
            pagesControl.AddPage(new OutputTypePage(Context.Instance));
            pagesControl.AddPage(new FinishingPage(Context.Instance));
            pagesControl.AddPage(new ExportingPage(Context.Instance));
            pagesControl.SelectPage("Welcome");
        }

        private void InitializeLocalization()
        {
            leftBarWelcomeLabel.Content = ResourceManager.GetString(
                "MainWindowLeftBarWelcome");
            leftBarOptionsLabel.Content = ResourceManager.GetString(
                "MainWindowLeftBarOptions");
            leftBarSkypeLabel.Content = ResourceManager.GetString(
                "MainWindowLeftBarSkype");
            leftBarExportingLabel.Content = ResourceManager.GetString(
                "MainWindowLeftBarExporting");
            leftBarFinishingLabel.Content = ResourceManager.GetString(
                "MainWindowLeftBarFinishing");
            confirmCloseString = ResourceManager.GetString(
                "MainWindowConfirmClose");

            Context.Instance.ResourceManager = ResourceManager;
            pagesControl.InitializeLocalization();
        }

        private void UpdateHeadingColor()
        {
            switch (Context.Instance.StatusCode)
            {
                case StatusCode.Finished:
                    headingColorGradientStop.Color = SuccessHeadingColor;
                    pagesControl.TitleFontColor = SuccessTitleColor;
                    break;
                case StatusCode.NoError:
                    // Do nothing.
                    break;
                default:
                    headingColorGradientStop.Color = FailureHeadingColor;
                    pagesControl.TitleFontColor = FailureTitleColor;
                    break;
            }
        }

        private void PagesControlActivePageChanged(object sender, 
            ActivePageChangedEventArgs e)
        {
            if (e.OldPage != null)
            {
                categoryToLabelCache[e.OldPage.Category].FontWeight =
                    FontWeights.Normal;
            }
            nextButton.IsEnabled = e.Page.NextButton != ButtonType.Disabled;
            if (e.Page.NextButton == ButtonType.Next)
            {
                nextButton.Content = Context.Instance.ResourceManager.GetString(
                    "MainWindowNext");
            }
            else if (e.Page.NextButton == ButtonType.Finish)
            {
                nextButton.Content = Context.Instance.ResourceManager.GetString(
                    "MainWindowFinish");
            }
            categoryToLabelCache[pagesControl.ActivePage.Category].FontWeight =
                FontWeights.Bold;
            UpdateHeadingColor();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (pagesControl.ActivePage.NextButton == ButtonType.Next)
            {
                pagesControl.GoNext();
            }
            else if (pagesControl.ActivePage.NextButton == ButtonType.Finish)
            {
                pagesControl.Finish();
            }
        }

        private void HyperlinkRequestNavigate(object sender, 
            System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Logger.Info("Navigate to {0}", e.Uri.AbsoluteUri);
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = Context.Instance.IsExportingInProgress &&
                Microsoft.Windows.Controls.MessageBox.Show(this,
                confirmCloseString, "Skype Historian",
                MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.No;
            Logger.Info("User have chosen to exit: {0}.", !e.Cancel);
        }
    }
}   
