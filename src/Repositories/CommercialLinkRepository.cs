using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class CommercialLinkRepository : CrudRepository<CommercialLink>
    {
        public CommercialLinkRepository(AppContext ctxt) : base(ctxt)
        { }

        public void NullifyClient(int clientId)
        {
            Entities
                .Where(cl => cl.ClientId == clientId)
                .ForEach(cl => cl.ClientId = null);
            SaveChanges();
        }

        public void NullifyTrader(int traderId)
        {
            Entities
                .Where(cl => cl.TraderId == traderId)
                .ForEach(cl => cl.TraderId = null);
            SaveChanges();
        }

        public void SeekReferences(CommercialLink cl)
        {
            Entry(cl).Reference("Client").Load();
            Entry(cl).Reference("Trader").Load();
        }

        public void CollectPurchases(CommercialLink cl)
        {
            Entry(cl).Collection("Purchase").Load();
        }
    }
}
