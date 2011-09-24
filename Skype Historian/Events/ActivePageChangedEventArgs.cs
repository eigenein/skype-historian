using System;
using SkypeHistorian.Controls.Pages;

namespace SkypeHistorian.Events
{
    public class ActivePageChangedEventArgs : EventArgs
    {
        private readonly AbstractPage oldPage;

        private readonly AbstractPage page;

        public ActivePageChangedEventArgs(AbstractPage oldPage,
            AbstractPage page)
        {
            this.oldPage = oldPage;
            this.page = page;
        }

        public AbstractPage OldPage
        {
            get { return oldPage; }
        }

        public AbstractPage Page
        {
            get { return page; }
        }
    }
}
