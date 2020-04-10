using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class OfferRepository : CrudRepository<Offer>
    {
        public OfferRepository(AppContext ctxt, Utils utils) : base(ctxt, utils)
        { }

        public int NullifyTrader(int traderId)
        {
            List<Offer> toUpdate = Entities.Where(offer => offer.TraderId == traderId).ToList();
            foreach (Offer offer in toUpdate)
            {
                offer.TraderId = null;
            }
            SaveChanges();
            return toUpdate.Count;
        }
    }
}
