using System;
using System.Collections.Generic;

namespace fidelizPlus_back.AppModel
{
    public partial class User : Entity
    {
        public User()
        {
            Client = new HashSet<Client>();
            Trader = new HashSet<Trader>();
        }

        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreationTime { get; set; }

        public virtual ICollection<Client> Client { get; set; }
        public virtual ICollection<Trader> Trader { get; set; }
    }
}
