using System;
using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;

    public class MultiService
    {
        private Utils Utils { get; set; }
        private FiltersHandler FiltersHandler { get; }
        private ClientService ClientService { get; }
        private TraderService TraderService { get; }
        private CommercialLinkService ClService { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }
        private AccountService<ClientAccount, ClientAccountDTO> ClientAccountService { get; }
        private AccountService<TraderAccount, TraderAccountDTO> TraderAccountService { get; }
        private PaymentMonitor PaymentMonitor { get; }

        public MultiService(
            Utils utils,
            FiltersHandler filtersHandler,
            ClientService clientService,
            TraderService traderService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            AccountService<ClientAccount, ClientAccountDTO> clientAccountService,
            AccountService<TraderAccount, TraderAccountDTO> traderAccountService,
            PaymentMonitor paymentMonitor
        )
        {
            Utils = utils;
            ClService = clService;
            ClientService = clientService;
            TraderService = traderService;
            FiltersHandler = filtersHandler;
            PurchaseService = purchaseService;
            ClientAccountService = clientAccountService;
            TraderAccountService = traderAccountService;
            PaymentMonitor = paymentMonitor;
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
                ret = Utils.ApplyFilter(ret, filter);
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
                ret = Utils.ApplyFilter(ret, filter);
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

        public (PurchaseDTO, int) SavePurchase(int clientId, int traderId, decimal amount)
        {
            (CommercialLink cl, _, _) = FindOrCreateCl(clientId, traderId);
            Purchase purchase = PurchaseService.Save(new Purchase()
            {
                CommercialLinkId = cl.Id,
                PayingTime = null,
                Amount = amount
            });
            return (PurchaseService.EntityToDTO(purchase), purchase.Id);
        }

        public PurchaseDTO Pay(int clientId, int purchaseId)
        {
            Purchase purchase = PurchaseService.FindEntity(purchaseId);
            if (purchase.PayingTime != null)
            {
                throw new AppException("Already payed", 400);
            }
            decimal amount = purchase.Amount;
            ClientAccount source = ClientService.GetAccount(clientId);
            if (amount > source.Balance)
            {
                throw new AppException("Not enough money", 400);
            }
            PurchaseService.SeekReferences(purchase);
            TraderAccount target = TraderService.GetAccount(purchase.CommercialLink.TraderId);
            PaymentMonitor.Remove(purchaseId);
            source.Balance -= amount;
            target.Balance += amount;
            purchase.PayingTime = DateTime.Now;
            ClientAccountService.Update(source);
            TraderAccountService.Update(target);
            purchase = PurchaseService.Update(purchase);
            return PurchaseService.EntityToDTO(purchase);
        }
    }
}
