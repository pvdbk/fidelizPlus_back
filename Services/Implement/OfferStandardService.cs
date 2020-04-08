namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;
    using Repositories;

    public class OfferStandardService : CrudStandardService<Offer, OfferDTO>, OfferService
    {
        private OfferRepository OfferRepo { get; }

        public OfferStandardService(
            OfferRepository repo,
            Utils utils,
            FiltersHandler filtersHandler
        ) : base(repo, utils, filtersHandler)
        {
            OfferRepo = repo;
        }

        public void NullifyTrader(int traderId)
        {
            OfferRepo.NullifyTrader(traderId);
        }
    }
}
