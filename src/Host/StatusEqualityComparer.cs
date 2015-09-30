using System.Collections.Generic;

namespace DG.TwitterClient.Host
{
    public class StatusEqualityComparer : EqualityComparer<DGStatus>
    {
        public override bool Equals(DGStatus x, DGStatus y)
        {
            return x.StatusID == y.StatusID;
        }

        public override int GetHashCode(DGStatus obj)
        {
            return obj.StatusID.GetHashCode();
        }
    }
}
