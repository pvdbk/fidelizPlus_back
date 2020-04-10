namespace fidelizPlus_back.AppDomain
{
    public class PurchaseComment : Entity
    {
        public int CommercialLinkId { get; set; }
        public virtual CommercialLink CommercialLink { get; set; }
    }
}
