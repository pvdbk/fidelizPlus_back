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

        public override IEnumerable<string> Fields => new string[] { "Id", "TraderId", "StartTime", "EndTime", "ContentPath" };
        public int? TraderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ContentPath { get; set; }

        public virtual Trader Trader { get; set; }
        public virtual ICollection<ClientOffer> ClientOffer { get; set; }
    }
}
