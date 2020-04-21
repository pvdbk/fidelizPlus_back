namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class OfferService : CrudService<Offer, OfferDTO>
    {
        private OfferRepository OfferRepo { get; }

        public OfferService(OfferRepository repo) : base(repo)
        {
            OfferRepo = repo;
        }

        public void NullifyTrader(int traderId)
        {
            OfferRepo.NullifyTrader(traderId);
        }
    }
}
