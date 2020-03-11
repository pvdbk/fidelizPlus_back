using System;
using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Offer : Entity
    {
        public Offer()
        {
            ClientOffer = new HashSet<ClientOffer>();
        }

        public int? TraderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ContentPath { get; set; }

        public virtual Trader Trader { get; set; }
        public virtual ICollection<ClientOffer> ClientOffer { get; set; }
    }
}
