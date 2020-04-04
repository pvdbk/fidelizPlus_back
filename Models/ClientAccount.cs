namespace fidelizPlus_back.Models
{
    public partial class ClientAccount : Entity
    {
        public int ClientId { get; set; }
        public string ExternalAccount { get; set; }
        public decimal Balance { get; set; }

        public virtual Client Client { get; set; }
    }
}
