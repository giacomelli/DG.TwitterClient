using System;

namespace DG.TwitterClient.Host
{
    public class StatusEventArgs : EventArgs
    {
        #region Construtores
        public StatusEventArgs(DGStatus status)
        {
            Status = status;
        }
        #endregion

        #region Propriedades
        public DGStatus Status
        {
            get;
            private set;
        }
        #endregion
    }
}
