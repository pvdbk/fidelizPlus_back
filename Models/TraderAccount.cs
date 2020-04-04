namespace fidelizPlus_back.Models
{
    public partial class TraderAccount : Entity
    {
        public int TraderId { get; set; }
        public string ExternalAccount { get; set; }

        public virtual Trader Trader { get; set; }
    }
}
