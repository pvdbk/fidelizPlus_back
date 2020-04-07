using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;

    public interface TraderService : CrudService<Trader, TraderDTO>
    {
        public IEnumerable<TraderAccountDTO> Accounts(int id, string filter);

        public TraderAccountDTO FindAccount(int traderId, int accountId);

        public IEnumerable<ClientDTOForTrader> Clients(int id, string filter);
    }
}
