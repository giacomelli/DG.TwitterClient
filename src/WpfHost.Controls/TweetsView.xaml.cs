#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using DG.TwitterClient.Host;
using DG.TwitterClient.WpfHost.Helpers;
using LinqToTwitter;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
#endregion

namespace DG.TwitterClient.WpfHost.Controls
{
    /// <summary>
    /// Interaction logic for TweetsView.xaml
    /// </summary>
    public sealed partial class TweetsView : UserControl
    {
        #region Eventos
        public event EventHandler TweetRead;
        #endregion

        #region Campos
        private List<DGStatus> m_statusList = new List<DGStatus>();
        private List<Account> m_activeAccounts = new List<Account>();
        private DateTime m_lastTimeTweetMouseEnter;
        #endregion

        public TweetsView()
        {
            InitializeComponent();

            m_activeAccounts = new List<Account>();
            CommunicationManager.StatusReceived += new EventHandler<StatusEventArgs>(CommunicationManager_TwitterStatusReceived);
            CommunicationManager.StatusUpdated += new EventHandler<StatusUpdatedEventArgs>(CommunicationManager_StatusUpdated);
        }

        void CommunicationManager_StatusUpdated(object sender, StatusUpdatedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {                
                VisualHelper.BindTarget(TweetsListView, ListViewItem.OpacityProperty);
            }));
        }

        public int UnreadTweetsCount
        {
            get;
            set;
        }        

        void CommunicationManager_TwitterStatusReceived(object sender, StatusEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                lock (this)
                {
                    if (m_statusList.Count == 0 || m_statusList.All(s => !s.StatusID.Equals(e.Status.StatusID))) {
                        m_statusList.Add(e.Status);
                        TweetsListView.Items.Insert(0, e.Status);
                        UnreadTweetsCount++;
                    }
                }
            }));
        }

        private void OnTweetRead(EventArgs e)
        {
            if (TweetRead != null)
            {
                TweetRead(this, e);
            }
        }

        protected void HandleItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lvi = (ListViewItem)e.Source;
            var status = (DGStatus)lvi.DataContext;

            var url = String.Format(CultureInfo.InvariantCulture, "http://twitter.com/{0}/status/{1}", status.User.Identifier.ScreenName, status.StatusID);
            Process.Start(url);
        }

        protected void HandleItemSelected(object sender, RoutedEventArgs e)
        {
            MarkAsRead(e);
        }
        
        private void StatusTextBlock_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {            
            MarkAsRead(e);
            Process.Start(e.Uri.OriginalString);
        }

        protected void HandleItemMouseEnter(object sender, MouseEventArgs e)
        {
            m_lastTimeTweetMouseEnter = DateTime.Now;
        }

        protected void HandleItemMouseLeave(object sender, MouseEventArgs e)
        {
            var diff = DateTime.Now - m_lastTimeTweetMouseEnter;

            if (diff.TotalSeconds >= 5)
            {
                MarkAsRead(e);
            }            
        }

        private void MarkAsRead(RoutedEventArgs tweetEventArgs)
        {
            var status = (DGStatus)VisualHelper.GetDataContext(tweetEventArgs.Source);
            status.Read = true;
        }
    }
}
