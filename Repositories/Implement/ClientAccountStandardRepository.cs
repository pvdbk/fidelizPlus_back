using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class ClientAccountStandardRepository : CrudStandardRepository<ClientAccount>, ClientAccountRepository
    {
        public ClientAccountStandardRepository(
            Context ctxt,
            FiltersHandler filtersHandler,
            Utils utils
        ) : base(ctxt, filtersHandler, utils)
        {
        }

        public int DeleteClient(int clientId)
        {
            List<int> toDelete = this.Entities
                .Where(account => account.ClientId == clientId)
                .Select(account => account.Id)
                .ToList();
            foreach (int accountId in toDelete)
            {
                this.Delete(accountId);
            }
            this.SaveChanges();
            return toDelete.Count;
        }
    }
}
