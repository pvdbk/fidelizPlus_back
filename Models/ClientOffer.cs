using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class ClientOffer : Entity
    {
        public override IEnumerable<string> Fields => new string[] { "Id", "ClientId", "OfferId", "UsesCount", "SentCount" };
        public int? ClientId { get; set; }
        public int OfferId { get; set; }
        public int UsesCount { get; set; }
        public int SentCount { get; set; }

        public virtual Client Client { get; set; }
        public virtual Offer Offer { get; set; }
    }
}
