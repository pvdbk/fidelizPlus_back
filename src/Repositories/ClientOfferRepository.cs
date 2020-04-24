using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class ClientOfferRepository : CrudRepository<ClientOffer>
    {
        public ClientOfferRepository(AppContext ctxt) : base(ctxt)
        { }

        public void NullifyClient(int clientId)
        {
            Entities
                .Where(co => co.ClientId == clientId)
                .ForEach(co => co.ClientId = null);
            SaveChanges();
        }
    }
}
