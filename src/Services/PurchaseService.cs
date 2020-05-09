namespace fidelizPlus_back.Services
{
	using AppDomain;
	using Repositories;

	public class PurchaseService : RelatedToBothService<Purchase, PurchaseDTO>
	{
		public PurchaseService(PurchaseRepository repo): base(repo)
		{ }
	}
}