namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Repositories;

    public class ClientService : UserEntityService<Client, ClientDTO, ClientAccount, ClientAccountDTO>
    {
        private ClientOfferService ClientOfferService { get; }

        public ClientService(
            UserEntityRepository<Client, ClientAccount> repo,
            CrudService<User, ClientDTO> userService,
            AccountService<ClientAccount, ClientAccountDTO> accountService,
            CommercialLinkService clService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            ClientOfferService clientOfferService
        ) : base(repo, userService, accountService, clService, purchaseService)
        {
            ClientOfferService = clientOfferService;
            NotRequiredForSaving = new string[] { "AdminPassword" };
            NotRequiredForUpdating = new string[] { "AdminPassword" };
        }

        public override void Delete(int id)
        {
            Client client = FindEntity(id);
            SeekReferences(client);
            ClService.NullifyClient(id);
            ClientOfferService.NullifyClient(id);
            Repo.Delete(id);
            UserService.Delete(client.Id);
            AccountService.Delete(client.AccountId);
        }
    }
}
