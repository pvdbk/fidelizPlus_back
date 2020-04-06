using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class TraderAccountStandardRepository : CrudStandardRepository<TraderAccount>, TraderAccountRepository
    {
        public TraderAccountStandardRepository(
            Context ctxt,
            FiltersHandler filtersHandler,
            Utils utils
        ) : base(ctxt, filtersHandler, utils)
        {
        }

        public int DeleteTrader(int traderId)
        {
            List<int> toDelete = this.Entities
                .Where(account => account.TraderId == traderId)
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
