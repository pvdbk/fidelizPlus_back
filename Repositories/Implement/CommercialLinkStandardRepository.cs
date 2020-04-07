using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Errors;
    using Models;

    public class CommercialLinkStandardRepository : CrudStandardRepository<CommercialLink>, CommercialLinkRepository
    {
        public CommercialLinkStandardRepository(
            Error error,
            AppContext ctxt,
            FiltersHandler filtersHandler,
            Utils utils
        ) : base(error, ctxt, filtersHandler, utils)
        {
        }

        public int NullifyClient(int clientId)
        {
            List<CommercialLink> toUpdate = this.Entities.Where(cl => cl.ClientId == clientId).ToList();
            foreach (CommercialLink cl in toUpdate)
            {
                cl.ClientId = null;
            }
            this.SaveChanges();
            return toUpdate.Count;
        }

        public int NullifyTrader(int traderId)
        {
            List<CommercialLink> toUpdate = this.Entities.Where(cl => cl.TraderId == traderId).ToList();
            foreach (CommercialLink cl in toUpdate)
            {
                cl.TraderId = null;
            }
            this.SaveChanges();
            return toUpdate.Count;
        }
    }
}
