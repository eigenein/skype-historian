using System;

namespace SkypeHistorian.Events
{
    public class PageChangeRequestedEventArgs : EventArgs
    {
        private readonly string page;

        public PageChangeRequestedEventArgs(string page)
        {
            this.page = page;
        }

        public string Page
        {
            get { return page; }
        }
    }
}
