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
    public partial class UsersView : UserControl
    {
        #region Constantes
        public int MaxUsers = 20;
        #endregion

        public UsersView()
        {            
            InitializeComponent();
            Users = new List<DGUser>();            
        }

        public List<DGUser> Users
        {
            get;
            set;
        }

        public void AddUser(DGUser user)
        {
            if (Users.Count(u => u.Identifier.ScreenName.Equals(user.Identifier.ScreenName, StringComparison.OrdinalIgnoreCase)) > 0)
            {                
                Users.Remove(user);
            }
            
            if (Users.Count == MaxUsers)
            {
                var oldestUser = Users[MaxUsers - 1];
                oldestUser.Visible = false;
                Users.Remove(oldestUser);
            }

            Users.Insert(0, user);

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                ListViewUsers.DataContext = null;
                ListViewUsers.DataContext = Users;  
            }));                        
        }

        protected void HandleItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lvi = (ListViewItem)e.Source;
            var user = (DGUser)lvi.DataContext;            
            var url = String.Format(CultureInfo.InvariantCulture, "http://twitter.com/{0}", user.Identifier.ScreenName);
            Process.Start(url);
        }

        protected void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)e.Source;
            var user = (DGUser)element.DataContext;
            user.Highlight = true;
        }

        protected void HandleMouseLeave(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)e.Source;
            var user = (DGUser)element.DataContext;
            user.Highlight = false;
        }      
    }
}