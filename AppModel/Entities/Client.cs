using System.Collections.Generic;

namespace fidelizPlus_back.AppModel
{
    public partial class Client : UserEntity<ClientAccount>
    {
        public Client()
        {
            ClientOffer = new HashSet<ClientOffer>();
            CommercialLink = new HashSet<CommercialLink>();
        }

        public string AdminPassword { get; set; }

        public virtual ICollection<ClientOffer> ClientOffer { get; set; }
    }
}
