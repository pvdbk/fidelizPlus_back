using System;
using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class CommercialLinkService : CrudService<CommercialLink, CommercialLink>
    {
        private CommercialLinkRepository ClRepo { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }

        public CommercialLinkService(
            CommercialLinkRepository repo,
            Utils utils,
             RelatedToBothService<Purchase, PurchaseDTO> purchaseService
        ) : base(repo, utils)
        {
            ClRepo = repo;
            PurchaseService = purchaseService;
        }

        public override CommercialLink EntityToDTO(CommercialLink entity)
        {
            return entity;
        }

        public override CommercialLink DTOToEntity(CommercialLink dto)
        {
            return dto;
        }

        public void SeekReferences(CommercialLink cl)
        {
            ClRepo.SeekReferences(cl);
        }

        public void NullifyClient(int clientId)
        {
            ClRepo.NullifyClient(clientId);
        }

        public void NullifyTrader(int traderId)
        {
            ClRepo.NullifyTrader(traderId);
        }

        public CommercialRelation GetClStatus(CommercialLink cl)
        {
            SeekReferences(cl);
            Tree filter = new Tree();
            filter.Add(new Tree($"{cl.ClientId}", "clientId"));
            filter.Add(new Tree("null", "payingTime"));
            IEnumerable<decimal> amounts = PurchaseService
                    .GetEntities(filter)
                    .Select(purchase => purchase.Amount);
            return new CommercialRelation()
            {
                Bookmark = Utils.GetBit(cl.Status, CommercialLink.BOOKMARK),
                Debt = amounts.Aggregate(0m, (x, y) => x + y)
            };
        }

        public CommercialLink FindWithBoth(int clientId, int traderId)
        {
            Tree filterTree = Utils.ToTree(new { clientId, traderId });
            Func<CommercialLink, bool> filterFunc = Utils.TreeToTest<CommercialLink>(filterTree);
            return Repo
                .GetEntities(filterFunc)
                .FirstOrDefault();
        }
    }
}
