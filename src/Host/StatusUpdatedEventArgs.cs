using System;

namespace DG.TwitterClient.Host
{
    public class StatusUpdatedEventArgs : EventArgs
    {
        public StatusUpdatedEventArgs(DGStatus[] statusReceived)
        {
            StatusReceived = statusReceived;
        }

        public DGStatus[] StatusReceived
        {
            get;
            private set;
        }
    }
}
