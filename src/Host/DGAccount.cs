using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using System.ComponentModel;

namespace DG.TwitterClient.Host
{
    public class DGAccount : INotifyPropertyChanged
    {
        #region Campos
        private bool m_visible;
        private bool m_highlight;
        #endregion

        public DGAccount(Account source)
        {
            Source = source;
            User = DGUser.GetUser(source.User);
            m_visible = true;
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

        public DGUser User
        {
            get;
            private set;
        }

        private Account Source
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
