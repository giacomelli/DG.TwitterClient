#region Usings
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DG.TwitterClient.Host;
using DG.TwitterClient.WpfHost.Controls;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;
using LinqToTwitter;
using DG.TwitterClient.WpfHost.Helpers;
#endregion

namespace WpfHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CommunicationManager.Initialize();
            CommunicationManager.LoginStarting += new EventHandler(CommunicationManager_LoginStarting);
            CommunicationManager.LoginFailed += new EventHandler<LoginFailedEventArgs>(CommunicationManager_LoginFailed);
            CommunicationManager.LoginFinished += new EventHandler(CommunicationManager_LoginFinished);
            CommunicationManager.StatusUpdating += new EventHandler(CommunicationManager_StatusUpdating);
            CommunicationManager.StatusUpdateFailed += new EventHandler<StatusUpdateFailedEventArgs>(CommunicationManager_StatusUpdateFailed);
            CommunicationManager.StatusUpdated += new EventHandler<StatusUpdatedEventArgs>(CommunicationManager_StatusUpdated);
            CommunicationManager.StatusReceived += new EventHandler<StatusEventArgs>(CommunicationManager_StatusReceived);

            InitializeComponent();

            TweetsViewLastUpdates.TweetRead += new EventHandler(TweetsViewLastUpdates_TweetRead);            
        }       

        void CommunicationManager_StatusReceived(object sender, StatusEventArgs e)
        {            
            LastActiveFriends.AddUser(e.Status.User);
        }        

        void TweetsViewLastUpdates_TweetRead(object sender, EventArgs e)
        {
            UpdateTweetsInfo();
        }

        void CommunicationManager_LoginStarting(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                LabelStatus.Content = "Login starting...";
            }));
        }

        void CommunicationManager_LoginFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                LabelStatus.Content = "Login finished.";

                foreach (var account in CommunicationManager.Accounts)
                {
                    ActiveAccounts.AddAccount(account);
                }
            })); 
        }

        void CommunicationManager_LoginFailed(object sender, LoginFailedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                LabelStatus.Content = "Login failed: " + e.Exception.Message;
            })); 
        }        

        void CommunicationManager_StatusUpdating(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                LabelStatus.Content = "Please, wait. Updating...";
            }));
        }

        void CommunicationManager_StatusUpdateFailed(object sender, StatusUpdateFailedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                LabelStatus.Content = "Update failed: " + e.Exception.GetBaseException().Message;
            }));
        }

        void CommunicationManager_StatusUpdated(object sender, StatusUpdatedEventArgs e)
        {            
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (e.StatusReceived.Length > 0)
                {                  
                    var balloon = new TweetsBalloon();
                    balloon.Title = "Updates";
                    balloon.Text = e.StatusReceived.Length + " new tweets.";                     
                    tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);                    
                }

                LabelStatus.Content = String.Format("Last update at {0:HH:mm:ss} ({1} new tweets).", DateTime.Now, e.StatusReceived.Length);
                UpdateTweetsInfo();
            })); 
        }

        private void UpdateTweetsInfo()
        {
            LabelStatus.Content = String.Format(CultureInfo.CurrentUICulture, "{0} tweets unread.", TweetsViewLastUpdates.UnreadTweetsCount);
        }         

        private void SliderUpdateInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CommunicationManager.Initialized)
            {
                CommunicationManager.ChangeStatusUpdateInterval(Convert.ToInt32(e.NewValue));
            }
        }

        #region Window
        private void SaveUserPreferences()
        {
            var p = DG.TwitterClient.WpfHost.Properties.Settings.Default;

            if (WindowState == WindowState.Maximized)
            {
                p.WindowTop = RestoreBounds.Top;
                p.WindowLeft = RestoreBounds.Left;
                p.WindowHeight = RestoreBounds.Height;
                p.WindowWidth = RestoreBounds.Width;
                p.WindowMaximized = true;
            }
            else
            {
                p.WindowTop = Top;
                p.WindowLeft = Left;
                p.WindowHeight = Height;
                p.WindowWidth = Width;
                p.WindowMaximized = false;
            }

            p.UpdateInterval = SliderUpdateInterval.Value;

            p.Save();
        }

        private void RestoreWindowPosition()
        {
            // Restore the window to previous user preferences.
            var p = DG.TwitterClient.WpfHost.Properties.Settings.Default;
            this.Top = p.WindowTop;
            this.Left = p.WindowLeft;
            this.Height = p.WindowHeight;
            this.Width = p.WindowWidth;

            if (p.WindowMaximized)
            {
                WindowState = WindowState.Maximized;
            }            
        }

        private void RestoreUserPreferences()
        {
            var p = DG.TwitterClient.WpfHost.Properties.Settings.Default;
            SliderUpdateInterval.Value = p.UpdateInterval;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreWindowPosition();
            CommunicationManager.Login();
            RestoreUserPreferences();
            CommunicationManager.UpdateStatus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveUserPreferences();
        }
        #endregion
    }
}
