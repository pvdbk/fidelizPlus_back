using System.Linq;

namespace fidelizPlus_back.Repositories
{
	using AppDomain;

	public class OfferRepository : CrudRepository<Offer>
	{
		public OfferRepository(AppContext ctxt) : base(ctxt)
		{ }

		public void NullifyTrader(int traderId)
		{
			Entities
				.Where(offer => offer.TraderId == traderId)
				.ForEach(offer => offer.TraderId = null);
			SaveChanges();
		}
	}
}
