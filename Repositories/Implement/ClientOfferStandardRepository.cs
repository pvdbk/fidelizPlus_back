using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public class ClientOfferStandardRepository : CrudStandardRepository<ClientOffer>, ClientOfferRepository
    {
        public ClientOfferStandardRepository(AppContext ctxt, Utils Utils) : base(ctxt, Utils)
        { }

        public int NullifyClient(int clientId)
        {
            List<ClientOffer> toUpdate = Entities.Where(co => co.ClientId == clientId).ToList();
            foreach (ClientOffer co in toUpdate)
            {
                co.ClientId = null;
            }
            SaveChanges();
            return toUpdate.Count;
        }
    }
}
