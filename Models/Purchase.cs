using System;
using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Purchase : Entity
    {
        public override IEnumerable<string> Fields => new string[] { "Id", "CommercialLinkId", "PayingTime", "Amount" };
        public int CommercialLinkId { get; set; }
        public DateTime PayingTime { get; set; }
        public decimal Amount { get; set; }

        public virtual CommercialLink CommercialLink { get; set; }
    }
}
