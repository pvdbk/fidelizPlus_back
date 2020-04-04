using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class OfferStandardRepository : CrudStandardRepository<Offer>, OfferRepository
    {
        public OfferStandardRepository(Context ctxt) : base(ctxt)
        {
        }

        public int NullifyTrader(int traderId)
        {
            List<Offer> toUpdate = this.Entities.Where(offer => offer.TraderId == traderId).ToList();
            foreach (Offer offer in toUpdate)
            {
                offer.TraderId = null;
            }
            this.SaveChanges();
            return toUpdate.Count;
        }
    }
}
