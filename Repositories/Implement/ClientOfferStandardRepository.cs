using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class ClientOfferStandardRepository : StandardRepository<ClientOffer>, ClientOfferRepository
    {
        public ClientOfferStandardRepository(Context ctxt) : base(ctxt)
        {
        }

        public int NullifyClient(int clientId)
        {
            List<ClientOffer> toUpdate = this.Entities.Where(co => co.ClientId == clientId).ToList();
            foreach (ClientOffer co in toUpdate)
            {
                co.ClientId = null;
            }
            this.SaveChanges();
            return toUpdate.Count;
        }
    }
}
