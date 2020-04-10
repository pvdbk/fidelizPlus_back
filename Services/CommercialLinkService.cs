using System.Linq;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class CommercialLinkService : CrudService<CommercialLink, CommercialLink>
    {
        private CommercialLinkRepository ClRepo { get; }

        public CommercialLinkService(
            CommercialLinkRepository repo,
            Utils utils,
            FiltersHandler filtersHandler
        ) : base(repo, utils, filtersHandler)
        {
            ClRepo = repo;
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

        public CommercialLinkStatus GetClStatus(CommercialLink cl)
        {
            return new CommercialLinkStatus()
            {
                Bookmark = Utils.GetBit(cl.Status, CommercialLink.BOOKMARK)
            };
        }

        public CommercialLink FindWithBoth(int clientId, int traderId)
        {
            return Repo
                .FindAll()
                .Where(cl => cl.ClientId == clientId && cl.TraderId == traderId)
                .FirstOrDefault();
        }
    }
}
