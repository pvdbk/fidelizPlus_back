using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Errors;
    using Models;

    public class ClientOfferStandardRepository : CrudStandardRepository<ClientOffer>, ClientOfferRepository
    {
        public ClientOfferStandardRepository(
            Error error,
            AppContext ctxt,
            FiltersHandler filtersHandler,
            Utils utils
        ) : base(error, ctxt, filtersHandler, utils)
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
