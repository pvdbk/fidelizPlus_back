namespace fidelizPlus_back.AppDomain
{
    public partial class ClientOffer : Entity
    {
        public int? ClientId { get; set; }
        public int OfferId { get; set; }
        public int UsedCount { get; set; }
        public int ReceivedCount { get; set; }

        public virtual Client Client { get; set; }
        public virtual Offer Offer { get; set; }
    }
}
