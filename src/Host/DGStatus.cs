using System;
using LinqToTwitter;
using System.ComponentModel;
using System.Linq;

namespace DG.TwitterClient.Host
{
    public class DGStatus : INotifyPropertyChanged
    {
        #region Campos
        private bool m_visbile;
        private bool m_read;
        #endregion

        public DGStatus(Status source, DGAccount account)
        {
            m_visbile = true;
            Source = source;
            Account = account;            
            User = DGUser.GetUser(source.User);

            var mediaEntities = source.Entities.MediaEntities.Where(m => m.Type.Equals("photo", StringComparison.OrdinalIgnoreCase));

            if (mediaEntities.Any())
            {
                MainImageUrl = mediaEntities.First().MediaUrl;
                HasImages = true;
            }
        }               

        public DGAccount Account
        {
            get;
            private set;
        }

        private Status Source
        {
            get;
            set;
        }

        public string StatusID
        {
            get
            {
                return Source.StatusID;
            }
        }

        public string Text
        {
            get
            {
                return Source.Text;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return Source.CreatedAt.ToLocalTime();
            }
        }

        public bool Read
        {
            get
            {
                return m_read;
            }

            set
            {
                m_read = value;
                OnPropertyChanged("Read");
            }
        }

        public bool Visible
        {
            get
            {
                return m_visbile;
            }

            set
            {
                m_visbile = value;
                OnPropertyChanged("Visible");
            }
        }

        public DGUser User
        {
            get;
            private set;
        }


        public string MainImageUrl { get; private set; }
        public bool HasImages { get; private set; }

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
