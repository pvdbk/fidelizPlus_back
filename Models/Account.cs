using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Account : Entity
    {
        public override IEnumerable<string> Fields => new string[] { "Id", "ClientId", "Balance" };
        public int ClientId { get; set; }
        public decimal Balance { get; set; }

        public virtual Client Client { get; set; }
    }
}
