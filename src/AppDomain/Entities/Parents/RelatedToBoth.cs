namespace fidelizPlus_back.AppDomain
{
	public class RelatedToBoth : Entity
	{
		public int CommercialLinkId { get; set; }
		public virtual CommercialLink CommercialLink { get; set; }
	}
}
