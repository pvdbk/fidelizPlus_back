using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class CommercialLinkRepository : CrudRepository<CommercialLink>
    {
        public CommercialLinkRepository(AppContext ctxt, Utils Utils) : base(ctxt, Utils)
        { }

        public int NullifyClient(int clientId)
        {
            List<CommercialLink> toUpdate = Entities.Where(cl => cl.ClientId == clientId).ToList();
            foreach (CommercialLink cl in toUpdate)
            {
                cl.ClientId = null;
            }
            SaveChanges();
            return toUpdate.Count;
        }

        public int NullifyTrader(int traderId)
        {
            List<CommercialLink> toUpdate = Entities.Where(cl => cl.TraderId == traderId).ToList();
            foreach (CommercialLink cl in toUpdate)
            {
                cl.TraderId = null;
            }
            SaveChanges();
            return toUpdate.Count;
        }

        public void SeekReferences(CommercialLink cl)
        {
            Entry(cl).Reference("Client").Load();
            Entry(cl).Reference("Trader").Load();
        }
    }
}
