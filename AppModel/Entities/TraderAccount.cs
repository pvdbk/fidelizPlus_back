using System.Collections.Generic;

namespace fidelizPlus_back.AppModel
{
    public partial class TraderAccount : Account
    {
        public TraderAccount()
        {
            Trader = new HashSet<Trader>();
        }

        public string Gni { get; set; }

        public virtual ICollection<Trader> Trader { get; set; }
    }
}
