namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;
    using Repositories;

    public class ClientOfferStandardService : CrudStandardService<ClientOffer, ClientOfferDTO>, ClientOfferService
    {
        private ClientOfferRepository CoRepo { get; }

        public ClientOfferStandardService(
            ClientOfferRepository repo,
            Utils utils,
            FiltersHandler filtersHandler
        ) : base(repo, utils, filtersHandler)
        {
            CoRepo = repo;
        }

        public void NullifyClient(int clientId)
        {
            CoRepo.NullifyClient(clientId);
        }
    }
}
