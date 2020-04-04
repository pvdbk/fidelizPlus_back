using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Client : Entity, UserEntity
    {
        public Client()
        {
            ClientAccount = new HashSet<ClientAccount>();
            ClientOffer = new HashSet<ClientOffer>();
            CommercialLink = new HashSet<CommercialLink>();
        }

        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public string AdminPassword { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ClientAccount> ClientAccount { get; set; }
        public virtual ICollection<ClientOffer> ClientOffer { get; set; }
        public virtual ICollection<CommercialLink> CommercialLink { get; set; }
    }
}
