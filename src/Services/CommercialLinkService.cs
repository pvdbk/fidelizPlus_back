using System.Linq;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Repositories;

    public class CommercialLinkService : CrudService<CommercialLink, CommercialLink>
    {
        private CommercialLinkRepository ClRepo { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }

        public CommercialLinkService(
            CommercialLinkRepository repo,
             RelatedToBothService<Purchase, PurchaseDTO> purchaseService
        ) : base(repo)
        {
            ClRepo = repo;
            PurchaseService = purchaseService;
        }

        public override CommercialLink EntityToDTO(CommercialLink entity) => entity;

        public override CommercialLink DTOToEntity(CommercialLink dto) => dto;

        public void SeekReferences(CommercialLink cl) => ClRepo.SeekReferences(cl);

        public void CollectPurchases(CommercialLink cl) => ClRepo.CollectPurchases(cl);

        public void NullifyClient(int clientId) => ClRepo.NullifyClient(clientId);


        public void NullifyTrader(int traderId) => ClRepo.NullifyTrader(traderId);

        public CommercialRelation GetClStatus(CommercialLink cl) =>
            new CommercialRelation()
            {
                Bookmark = cl.Status.GetBit(CommercialLink.BOOKMARK),
                Debt = PurchaseService.Entities
                    .Where(purchase => purchase.CommercialLinkId == cl.Id && purchase.PayingTime == null)
                    .Select(purchase => purchase.Amount)
                    .ToList()
                    .Aggregate(0m, (x, y) => x + y)
            };

        public CommercialLink FindWithBoth(int clientId, int traderId) =>
            Entities
                .Where(cl => cl.ClientId == clientId && cl.TraderId == traderId)
                .FirstOrDefault();
    }
}
