using System;
using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
	using AppDomain;
	using Payment;
	using Identification;

	public class MultiService
	{
		private ClientService ClientService { get; }
		private TraderService TraderService { get; }
		private CommercialLinkService ClService { get; }
		private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }
		private AccountService<ClientAccount, ClientAccountDTO> ClientAccountService { get; }
		private AccountService<TraderAccount, TraderAccountDTO> TraderAccountService { get; }
		private PaymentMonitor PaymentMonitor { get; }
		private Credentials Credentials { get; }

		public MultiService(
			ClientService clientService,
			TraderService traderService,
			CommercialLinkService clService,
			RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
			AccountService<ClientAccount, ClientAccountDTO> clientAccountService,
			AccountService<TraderAccount, TraderAccountDTO> traderAccountService,
			PaymentMonitor paymentMonitor,
			Credentials credentials
		)
		{
			ClService = clService;
			ClientService = clientService;
			TraderService = traderService;
			PurchaseService = purchaseService;
			ClientAccountService = clientAccountService;
			TraderAccountService = traderAccountService;
			PaymentMonitor = paymentMonitor;
			Credentials = credentials;
		}

		private (CommercialLink, Client, Trader) FindOrCreateCl(int clientId, int traderId)
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

		public IEnumerable<TraderForClients> TradersForClient(int id, string filter)
		{
			ClientService.CheckCredentials(id);
			Client client = ClientService.FindEntity(id);
			ClientService.CollectCl(client);
			var ret = new List<TraderForClients>();
			Func<TraderForClients, bool> test = filter.ToTest<TraderForClients>();
			foreach (CommercialLink cl in client.CommercialLink)
			{
				PrivateTrader privateTrader = TraderService.EntityToDTO(cl.Trader);
				TraderForClients traderForClient = privateTrader.CastAs<TraderForClients>();
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
			TraderService.CheckCredentials(id);
			Trader trader = TraderService.FindEntity(id);
			TraderService.CollectCl(trader);
			var ret = new List<ClientForTraders>();
			Func<ClientForTraders, bool> test = filter.ToTest<ClientForTraders>();
			foreach (CommercialLink cl in trader.CommercialLink)
			{
				PrivateClient privateClient = ClientService.EntityToDTO(cl.Client);
				ClientForTraders clientForTrader = privateClient.CastAs<ClientForTraders>();
				clientForTrader.CommercialRelation = ClService.GetClStatus(cl);
				if (test(clientForTrader))
				{
					ret.Add(clientForTrader);
				}
			}
			return ret;
		}

		public TraderForClients MarkTrader(int clientId, int traderId, bool? bookMark)
		{
			ClientService.CheckCredentials(clientId);
			if (bookMark == null)
			{
				throw new Break("Missing parameter in query: bookMark (boolean)", BreakCode.ErrQuery, 400);
			}
			(CommercialLink cl, _, Trader trader) = FindOrCreateCl(clientId, traderId);
			cl.Status = cl.Status.SetBit(CommercialLink.BOOKMARK, (bool)bookMark);
			ClService.QuickUpdate(cl.Id, cl);
			PrivateTrader dto = TraderService.EntityToDTO(cl.Trader);
			TraderForClients extended = dto.CastAs<TraderForClients>();
			extended.CommercialRelation = ClService.GetClStatus(cl);
			return extended;
		}

		public (PurchaseDTO, int) SavePurchase(int clientId, int traderId, decimal amount)
		{
			decimal amnt = (decimal)(int)(amount*100) / 100;
			if(amnt <= 0)
			{
				throw new Break("A strictly positive amount must be specified", BreakCode.ErrQuery, 400);
			}
			TraderService.CheckCredentials(traderId);
			(CommercialLink cl, _, _) = FindOrCreateCl(clientId, traderId);
			Purchase purchase = PurchaseService.Save(new Purchase()
			{
				CommercialLinkId = cl.Id,
				PayingTime = null,
				Amount = amnt
			});
			return (PurchaseService.EntityToDTO(purchase), purchase.Id);
		}

		public PurchaseDTO Pay(int clientId, int purchaseId)
		{
			ClientService.CheckCredentials(clientId);
			Purchase purchase = PurchaseService.FindEntity(purchaseId);
			if (purchase.PayingTime != null)
			{
				throw new Break($"Already payed : {purchaseId}", BreakCode.AlreadyPayed, 400);
			}
			decimal amount = purchase.Amount;
			ClientAccount source = ClientService.GetAccount(clientId);
			if (amount > source.Balance)
			{
				throw new Break("Not enough money", BreakCode.ErrAccount, 400);
			}
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
