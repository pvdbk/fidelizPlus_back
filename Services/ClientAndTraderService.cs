using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;

    public class ClientAndTraderService
    {
        private Utils Utils { get; set; }
        private FiltersHandler FiltersHandler { get; }
        private ClientService ClientService { get; }
        private TraderService TraderService { get; }
        private CommercialLinkService ClService { get; }

        public ClientAndTraderService(
            Utils utils,
            FiltersHandler filtersHandler,
            ClientService clientService,
            TraderService traderService,
            CommercialLinkService clService
        )
        {
            Utils = utils;
            ClService = clService;
            ClientService = clientService;
            TraderService = traderService;
            FiltersHandler = filtersHandler;
        }

        public IEnumerable<ExtendedTraderDTO> TradersForClient(int id, string filter)
        {
            Client client = ClientService.FindEntity(id);
            ClientService.CollectCl(client);
            foreach (CommercialLink cl in client.CommercialLink)
            {
                ClService.SeekReferences(cl);
            }
            IEnumerable<Trader> traders = client.CommercialLink.Select(cl => cl.Trader);
            IEnumerable<TraderDTO> traderDTOs = traders.Select(trader => TraderService.EntityToDTO(trader));
            IEnumerable<ExtendedTraderDTO> ret = traderDTOs.Select(dto => TraderService.ExtendDTO(dto, id));
            if (filter != null)
            {
                ret = FiltersHandler.Apply(ret, new Tree(filter));
            }
            return ret;
        }

        public IEnumerable<ExtendedClientDTO> ClientsForTrader(int id, string filter)
        {
            Trader trader = TraderService.FindEntity(id);
            TraderService.CollectCl(trader);
            foreach (CommercialLink cl in trader.CommercialLink)
            {
                ClService.SeekReferences(cl);
            }
            IEnumerable<Client> clients = trader.CommercialLink.Select(cl => cl.Client);
            IEnumerable<ClientDTO> clientDTOs = clients.Select(client => ClientService.EntityToDTO(client));
            IEnumerable<ExtendedClientDTO> ret = clientDTOs.Select(dto => ClientService.ExtendDTO(dto, id));
            if (filter != null)
            {
                ret = FiltersHandler.Apply(ret, new Tree(filter));
            }
            return ret;
        }

        public (CommercialLink, Client, Trader) FindOrCreateCl(int clientId, int traderId)
        {
            Client client = ClientService.FindEntity(clientId);
            Trader trader = TraderService.FindEntity(traderId);
            CommercialLink cl = ClService.FindWithBoth(clientId, traderId);
            if (cl == null)
            {
                cl = new CommercialLink()
                {
                    ClientId = clientId,
                    TraderId = traderId,
                    Status = CommercialLink.DEFAULT_STATUS
                };
                ClService.QuickSave(cl);
            }
            return (cl, client, trader);
        }

        public ExtendedTraderDTO MarkTrader(int clientId, int traderId, bool? bookMark)
        {
            if (bookMark == null)
            {
                throw new AppException("Missing parameters", 400);
            }
            (CommercialLink cl, _, Trader trader) = FindOrCreateCl(clientId, traderId);
            cl.Status = Utils.SetBit(cl.Status, CommercialLink.BOOKMARK, (bool)bookMark);
            ClService.QuickUpdate(cl.Id, cl);
            return TraderService.ExtendDTO(TraderService.EntityToDTO(trader), clientId);
        }
    }
}
