using System.Linq;
using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class TraderService : UserEntityService<Trader, TraderDTO, TraderAccount, TraderAccountDTO>
    {
        private OfferService OfferService { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }

        public TraderService(
            UserEntityRepository<Trader, TraderAccount> repo,
            Utils utils,
            CrudService<User, TraderDTO> userService,
            AccountService<TraderAccount, TraderAccountDTO> accountService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            OfferService offerService
        ) : base(repo, utils, userService, accountService, clService)
        {
            PurchaseService = purchaseService;
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

        public ExtendedTraderDTO ExtendDTO(TraderDTO dto, int clientId)
        {
            ExtendedTraderDTO ret = null;
            CommercialLink cl = null;
            if (dto.Id != null)
            {
                ret = Utils.Cast<ExtendedTraderDTO, TraderDTO>(dto);
                cl = ClService.FindWithBoth(clientId, (int)dto.Id);
            }
            if (cl == null)
            {
                throw new AppException("Bad use of ClientStandardService.ExtendDTO");
            }
            ret.CommercialRelation = ClService.GetClStatus(cl);
            return ret;
        }

        public IEnumerable<PurchaseDTO> Purchases(int id, string filter)
        {
            IEnumerable<PurchaseDTO> ret = PurchaseService.FilterOrFindAll(null).Where(purchase => purchase.TraderId == id);
            ret = Utils.ApplyFilter(ret, filter);
            return ret;
        }
    }
}
