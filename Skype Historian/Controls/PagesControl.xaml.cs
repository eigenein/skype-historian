using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using SkypeHistorian.Controls.Pages;
using SkypeHistorian.Events;

namespace SkypeHistorian.Controls
{
    /// <summary>
    /// Interaction logic for PagesControl.xaml
    /// </summary>
    public partial class PagesControl : UserControl
    {
        private AbstractPage activePage;

        private readonly Dictionary<string, AbstractPage> pages =
            new Dictionary<string, AbstractPage>();

        private static readonly Logger Logger =
            LogManager.GetCurrentClassLogger();

        public PagesControl()
        {
            InitializeComponent();
        }

        public void InitializeLocalization()
        {
            if (activePage != null)
            {
                activePage.InitializeLocalization();
                titleLabel.Content = activePage.Title;
            }
        }

        public event EventHandler<ActivePageChangedEventArgs> ActivePageChanged;

        private void OnActivePageChanged(AbstractPage oldPage, AbstractPage page)
        {
            EventHandler<ActivePageChangedEventArgs> handler = ActivePageChanged;
            if (handler != null)
            {
                handler(this, new ActivePageChangedEventArgs(oldPage, page));
            }
        }

        public AbstractPage ActivePage
        {
            get { return activePage; }
        }

        public void AddPage(AbstractPage page)
        {
            if (pages.ContainsKey(page.Id))
            {
                throw new InvalidOperationException("The Id is already here.");
            }
            pages.Add(page.Id, page);
        }

        public void SelectPage(string id)
        {
            contentGrid.Children.Clear();
            AbstractPage page;
            if (pages.TryGetValue(id, out page))
            {
                SelectPage(page);
            }
            else
            {
                Logger.Error("Page {0} is not found in pages dictionary.", id);
            }
        }

        private void SelectPage(AbstractPage page)
        {
            Logger.Info("Selecting page {0}", page.Id);
            AbstractPage oldPage = activePage;
            activePage = page;
            activePage.InitializeLocalization();
            activePage.PageChangeRequested += ActivePageChangeRequested;
            titleLabel.Content = activePage.Title;
            contentGrid.Children.Clear();
            contentGrid.Children.Add(activePage);
            OnActivePageChanged(oldPage, activePage);
            activePage.Initialize();
        }

        public void GoNext()
        {
            if (!activePage.OnNext())
            {
                return;
            }
            if (activePage.NextPageId == null)
            {
                throw new InvalidOperationException(
                    "There is no default next page.");
            }
            SelectPage(activePage.NextPageId);
        }

        public void Finish()
        {
            activePage.OnFinish();
        }

        private void ActivePageChangeRequested(object sender,
            PageChangeRequestedEventArgs e)
        {
            Logger.Info("Page {0} is requested.", e.Page);
            SelectPage(e.Page);
        }

        public Color TitleFontColor
        {
            set { titleBrush.Color = value; }
            get { return titleBrush.Color; }
        }
    }
}
