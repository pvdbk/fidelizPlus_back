using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using DTO;

    public interface ClientAndTraderService
    {
        public IEnumerable<ExtendedTraderDTO> TradersForClient(int id, string filter);

        public IEnumerable<ExtendedClientDTO> ClientsForTrader(int id, string filter);

        public ExtendedTraderDTO MarkTrader(int clientId, int traderId, bool? bookMark);
    }
}
