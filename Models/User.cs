﻿using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class User : Entity
    {
        public User()
        {
            Client = new HashSet<Client>();
            Trader = new HashSet<Trader>();
        }

        public override IEnumerable<string> Fields => new string[] { "Id", "Surname", "FirstName", "Email", "Password" };
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Client> Client { get; set; }
        public virtual ICollection<Trader> Trader { get; set; }
    }
}
