using System;

namespace fidelizPlus_back.AppDomain
{
    public partial class Purchase : PurchaseComment
    {
        public DateTime? PayingTime { get; set; }
        public decimal Amount { get; set; }
    }
}
