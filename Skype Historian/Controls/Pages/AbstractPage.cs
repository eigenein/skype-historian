using System;
using System.Windows.Controls;
using SkypeHistorian.Events;
using SkypeHistorian.Exporting;

namespace SkypeHistorian.Controls.Pages
{
    public class AbstractPage : UserControl
    {
        #region Abstract members

        public virtual string Id
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string Title
        {
            get { return String.Format("Page Id: {0}", Id); }
        }

        public virtual string Category
        {
            get { return Id; }
        }

        public virtual ButtonType NextButton
        {
            get { return ButtonType.Disabled; }
        }

        public virtual string NextPageId
        {
            get { return null; }
        }

        public virtual void InitializeLocalization()
        {
            // Do nothing.
        }

        public virtual void Initialize()
        {
            // Do nothing.
        }

        /// <summary>
        /// Invoked when the page is about to close by Next button press.
        /// </summary>
        /// <returns>Whether close is allowed.</returns>
        public virtual bool OnNext()
        {
            return true;
        }

        public virtual void OnFinish()
        {
            // Do nothing.
        }

        #endregion

        #region Base functionality

        protected readonly Context Context;

        /// <summary>
        /// Initializes a new instance for the designer.
        /// </summary>
        protected AbstractPage()
            : this(null)
        {
            // Do nothing.
        }

        protected AbstractPage(Context context)
        {
            this.Context = context;
        }

        public event EventHandler<PageChangeRequestedEventArgs> PageChangeRequested;

        protected void OnPageChangeRequested(string page)
        {
            EventHandler<PageChangeRequestedEventArgs> handler = PageChangeRequested;
            if (handler != null)
            {
                handler(this, new PageChangeRequestedEventArgs(page));
            }
        }

        #endregion
    }
}
