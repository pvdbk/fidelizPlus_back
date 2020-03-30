using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Client : Entity, UserEntity
    {
        public Client()
        {
            Account = new HashSet<Account>();
            ClientOffer = new HashSet<ClientOffer>();
            CommercialLink = new HashSet<CommercialLink>();
        }

        public override IEnumerable<string> Fields => new string[] { "Id", "UserId", "ConnectionId", "AdminPassword" };
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public string AdminPassword { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Account> Account { get; set; }
        public virtual ICollection<ClientOffer> ClientOffer { get; set; }
        public virtual ICollection<CommercialLink> CommercialLink { get; set; }
    }
}
