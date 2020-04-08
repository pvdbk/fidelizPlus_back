namespace fidelizPlus_back.AppModel
{
    public class PurchaseComment : Entity
    {
        public int CommercialLinkId { get; set; }
        public virtual CommercialLink CommercialLink { get; set; }
    }
}
