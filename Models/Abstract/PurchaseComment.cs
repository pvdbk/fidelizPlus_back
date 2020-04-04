namespace fidelizPlus_back.Models
{
    public interface PurchaseComment
    {
        public int CommercialLinkId { get; set; }
        public CommercialLink CommercialLink { get; set; }
    }
}
