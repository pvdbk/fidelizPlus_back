namespace fidelizPlus_back.Repositories
{
	using AppDomain;

	public class PurchaseRepository : RelatedToBothRepository<Purchase>
	{
		public PurchaseRepository(AppContext ctxt) : base(ctxt)
		{ }
	}
}