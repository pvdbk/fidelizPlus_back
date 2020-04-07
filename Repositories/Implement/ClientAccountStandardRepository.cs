using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Errors;
    using Models;

    public class ClientAccountStandardRepository : CrudStandardRepository<ClientAccount>, ClientAccountRepository
    {
        public ClientAccountStandardRepository(
            Error error,
            AppContext ctxt,
            FiltersHandler filtersHandler,
            Utils utils
        ) : base(error, ctxt, filtersHandler, utils)
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
