namespace fidelizPlus_back.Models
{
    public partial class Account : Entity
    {
        public int ClientId { get; set; }
        public decimal Balance { get; set; }

        public virtual Client Client { get; set; }
    }
}
