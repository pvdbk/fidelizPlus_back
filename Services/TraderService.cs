namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class TraderService : UserEntityService<Trader, TraderDTO, TraderAccount, TraderAccountDTO>
    {
        private OfferService OfferService { get; }

        public TraderService(
            UserEntityRepository<Trader, TraderAccount> repo,
            CrudService<User, TraderDTO> userService,
            AccountService<TraderAccount, TraderAccountDTO> accountService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            OfferService offerService
        ) : base(repo, userService, accountService, clService, purchaseService)
        {
            OfferService = offerService;
            NotRequiredForSaving = new string[] { "Address", "Phone", "LogoPath" };
            NotRequiredForUpdating = new string[] { "Address", "Phone", "LogoPath" };
        }

        public override void Delete(int id)
        {
            Trader trader = FindEntity(id);
            SeekReferences(trader);
            ClService.NullifyTrader(id);
            OfferService.NullifyTrader(id);
            Repo.Delete(id);
            UserService.Delete(trader.Id);
            AccountService.Delete(trader.AccountId);
        }

        public void DeletePurchase(int traderId, int purchaseId)
        {
            Trader trader = FindEntity(traderId);
            Purchase purchase = PurchaseService.FindEntity(purchaseId);
            PurchaseService.SeekReferences(purchase);
            if (purchase.CommercialLink.TraderId != traderId)
            {
                throw new AppException("Purchase not found", 404);
            }
            PurchaseService.Delete(purchaseId);
        }
    }
}
