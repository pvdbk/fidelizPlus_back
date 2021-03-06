﻿namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Repositories;
    using Identification;

    public class TraderService : UserEntityService<Trader, PrivateTrader, PublicTrader, TraderAccount, TraderAccountDTO>
    {
        private OfferService OfferService { get; }

        public TraderService(
            UserEntityRepository<Trader, TraderAccount> repo,
            CrudService<User, PrivateTrader> userService,
            AccountService<TraderAccount, TraderAccountDTO> accountService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            Credentials credentials,
            OfferService offerService
        ) : base(repo, userService, accountService, clService, purchaseService, credentials)
        {
            OfferService = offerService;
            NotRequiredForSaving = new string[] { "Address", "Phone", "LogoPath" };
            NotRequiredForUpdating = new string[] { "Address", "Phone", "LogoPath" };
        }

        public override void Delete(int id)
        {
            CheckCredentials(id);
            Trader trader = FindEntity(id);
            ClService.NullifyTrader(id);
            OfferService.NullifyTrader(id);
            Repo.Delete(id);
            UserService.Delete(trader.Id);
            AccountService.Delete(trader.AccountId);
        }

        public void DeletePurchase(int traderId, int purchaseId)
        {
            CheckCredentials(traderId);
            Trader trader = FindEntity(traderId);
            Purchase purchase = PurchaseService.FindEntity(purchaseId);
            if (purchase.CommercialLink.TraderId != traderId)
            {
                throw new AppException("Purchase not found", 404);
            }
            PurchaseService.Delete(purchaseId);
        }
    }
}
