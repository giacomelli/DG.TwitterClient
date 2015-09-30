#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using DG.TwitterClient.Host;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using System.Windows;
#endregion

namespace DG.TwitterClient.WpfHost.Controls
{
    /// <summary>
    /// Interaction logic for AccountsView.xaml
    /// </summary>
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {            
            InitializeComponent();
            Accounts = new List<DGAccount>();            
        }

        private List<DGAccount> Accounts
        {
            get;
            set;
        }

        public void AddAccount(DGAccount account)
        {
            if (Accounts.Count(a => a.User.Identifier.ScreenName.Equals(account.User.Identifier.ScreenName, StringComparison.OrdinalIgnoreCase)) == 0)
            {
                Accounts.Add(account);

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    ListViewAccounts.DataContext = null;
                    ListViewAccounts.DataContext = Accounts;
                }));
            }   
        }

        protected void HandleItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lvi = (ListViewItem)e.Source;
            var account = (DGAccount)lvi.DataContext;            
            var url = String.Format(CultureInfo.InvariantCulture, "http://twitter.com/{0}", account.User.Identifier.ScreenName);
            Process.Start(url);
        }

        protected void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)e.Source;
            var account = (DGAccount)element.DataContext;
            account.Highlight = true;
        }

        protected void HandleMouseLeave(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)e.Source;
            var account = (DGAccount)element.DataContext;
            account.Highlight = false;
        }
    }
}