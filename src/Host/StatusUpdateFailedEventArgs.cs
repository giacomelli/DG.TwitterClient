using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DG.TwitterClient.Host
{
    public class StatusUpdateFailedEventArgs : EventArgs
    {
        public StatusUpdateFailedEventArgs(Exception ex)
        {
            Exception = ex;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }
}
