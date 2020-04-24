using System;

namespace fidelizPlus_back.AppDomain
{
    public class PurchaseDTO : RelatedToBothDTO
    {
        public DateTime? PayingTime { get; set; }
        public decimal Amount { get; set; }
    }
}
