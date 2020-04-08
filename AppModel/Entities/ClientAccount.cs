using System.Collections.Generic;

namespace fidelizPlus_back.AppModel
{
    public partial class ClientAccount : Account
    {
        public ClientAccount()
        {
            Client = new HashSet<Client>();
        }

        public string ExternalAccount { get; set; }

        public virtual ICollection<Client> Client { get; set; }
    }
}
