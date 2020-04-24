using System;
using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Payment;

    public class MultiService
    {
        private ClientService ClientService { get; }
        private TraderService TraderService { get; }
        private CommercialLinkService ClService { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }
        private AccountService<ClientAccount, ClientAccountDTO> ClientAccountService { get; }
        private AccountService<TraderAccount, TraderAccountDTO> TraderAccountService { get; }
        private PaymentMonitor PaymentMonitor { get; }

        public MultiService(
            ClientService clientService,
            TraderService traderService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            AccountService<ClientAccount, ClientAccountDTO> clientAccountService,
            AccountService<TraderAccount, TraderAccountDTO> traderAccountService,
            PaymentMonitor paymentMonitor
        )
        {
            ClService = clService;
            ClientService = clientService;
            TraderService = traderService;
            PurchaseService = purchaseService;
            ClientAccountService = clientAccountService;
            TraderAccountService = traderAccountService;
            PaymentMonitor = paymentMonitor;
        }

        public IEnumerable<TraderForClients> TradersForClient(int id, string filter)
        {
            Client client = ClientService.FindEntity(id);
            ClientService.CollectCl(client);
            var ret = new List<TraderForClients>();
            Func<TraderForClients, bool> test = filter.ToTest<TraderForClients>();
            foreach (CommercialLink cl in client.CommercialLink)
            {
                ClService.SeekReferences(cl);
                TraderDTO traderDTO = TraderService.EntityToDTO(cl.Trader);
                TraderForClients traderForClient = traderDTO.CastAs<TraderForClients>();
                traderForClient.CommercialRelation = ClService.GetClStatus(cl);
                if (test(traderForClient))
                {
                    ret.Add(traderForClient);
                }
            }
            return ret;
        }

        public IEnumerable<ClientForTraders> ClientsForTrader(int id, string filter)
        {
            Trader trader = TraderService.FindEntity(id);
            TraderService.CollectCl(trader);
            var ret = new List<ClientForTraders>();
            Func<ClientForTraders, bool> test = filter.ToTest<ClientForTraders>();
            foreach (CommercialLink cl in trader.CommercialLink)
            {
                ClService.SeekReferences(cl);
                ClientDTO clientDTO = ClientService.EntityToDTO(cl.Client);
                ClientForTraders clientForTrader = clientDTO.CastAs<ClientForTraders>();
                clientForTrader.CommercialRelation = ClService.GetClStatus(cl);
                if (test(clientForTrader))
                {
                    ret.Add(clientForTrader);
                }
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

        public TraderForClients MarkTrader(int clientId, int traderId, bool? bookMark)
        {
            if (bookMark == null)
            {
                throw new AppException("Missing parameters", 400);
            }
            (CommercialLink cl, _, Trader trader) = FindOrCreateCl(clientId, traderId);
            cl.Status = cl.Status.SetBit(CommercialLink.BOOKMARK, (bool)bookMark);
            ClService.QuickUpdate(cl.Id, cl);
            TraderDTO dto = TraderService.EntityToDTO(cl.Trader);
            TraderForClients extended = dto.CastAs<TraderForClients>();
            extended.CommercialRelation = ClService.GetClStatus(cl);
            return extended;
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
