using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Trader : Entity
    {
        public Trader()
        {
            CommercialLink = new HashSet<CommercialLink>();
            Offer = new HashSet<Offer>();
        }

        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public string Label { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string LogoPath { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<CommercialLink> CommercialLink { get; set; }
        public virtual ICollection<Offer> Offer { get; set; }
    }
}
