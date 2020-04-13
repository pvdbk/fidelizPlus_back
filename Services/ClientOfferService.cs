namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class ClientOfferService : CrudService<ClientOffer, ClientOfferDTO>
    {
        private ClientOfferRepository CoRepo { get; }

        public ClientOfferService(ClientOfferRepository repo, Utils utils) : base(repo, utils)
        {
            CoRepo = repo;
        }

        public void NullifyClient(int clientId)
        {
            CoRepo.NullifyClient(clientId);
        }
    }
}
