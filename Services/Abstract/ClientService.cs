using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;

    public interface ClientService : CrudService<Client, ClientDTO>
    {
        public IEnumerable<ClientAccountDTO> Accounts(int id);

        public ClientAccountDTO FindAccount(int clientId, int accountId);

        public ClientAccountDTO UpdateAccount(int clientId, int accountId, int amount);

        public IEnumerable<TraderDTO> Traders(int id, string filter);

        public CommercialLinkDTO MarkTrader(int clientId, int traderId, bool bookMark);
    }
}
