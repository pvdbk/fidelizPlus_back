namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class ClientOfferService : CrudService<ClientOffer, ClientOfferDTO>
    {
        private ClientOfferRepository CoRepo { get; }

        public ClientOfferService(ClientOfferRepository repo) : base(repo)
        {
            CoRepo = repo;
        }

        public void NullifyClient(int clientId)
        {
            CoRepo.NullifyClient(clientId);
        }
    }
}
