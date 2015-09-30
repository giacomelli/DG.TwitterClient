using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using System.ComponentModel;

namespace DG.TwitterClient.Host
{
    public class DGUser : INotifyPropertyChanged
    {
        #region Campos
        private bool m_visible;
        private bool m_highlight;
        private static List<DGUser> m_users = new List<DGUser>();
        #endregion

        private DGUser(User source)
        {
            Source = source;
            m_visible = true;
        }

        public static DGUser GetUser(User user)
        {
            DGUser dg = m_users.FirstOrDefault(u => u.Identifier.ScreenName.Equals(user.Identifier.ScreenName, StringComparison.OrdinalIgnoreCase));

            if (dg == null)
            {
                dg = new DGUser(user);
                m_users.Add(dg);
            }

            return dg;
        }

        public UserIdentifier Identifier
        {
            get
            {
                return Source.Identifier;
            }
        }

        public string ProfileLinkColor
        {
            get
            {
                return Source.ProfileLinkColor;
            }
        }

        public string ProfileSidebarBorderColor
        {
            get
            {
                return Source.ProfileSidebarBorderColor;
            }
        }

        public string ProfileImageUrl
        {
            get
            {
                return Source.ProfileImageUrl;
            }
        }

        public string ProfileTextColor
        {
            get
            {
                return Source.ProfileTextColor;
            }
        }

        public bool Visible
        {
            get
            {
                return m_visible;
            }

            set
            {
                m_visible = value;
                OnPropertyChanged("Visible");
            }
        }

        public bool Highlight
        {
            get
            {
                return m_highlight;
            }

            set
            {
                m_highlight = value;
                OnPropertyChanged("Highlight");
            }
        }

        private User Source
        {
            get;
            set;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
