using System;

namespace fidelizPlus_back.Models
{
    public partial class Purchase : Entity
    {
        public int CommercialLinkId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public virtual CommercialLink CommercialLink { get; set; }
    }
}
