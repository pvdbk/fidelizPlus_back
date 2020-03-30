using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class CommercialLinkStandardRepository : StandardRepository<CommercialLink>, CommercialLinkRepository
    {
        public CommercialLinkStandardRepository(Context ctxt) : base(ctxt)
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
