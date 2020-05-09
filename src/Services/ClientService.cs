namespace fidelizPlus_back.Services
{
	using AppDomain;
	using Repositories;
	using Identification;

	public class ClientService : UserEntityService<Client, PrivateClient, PublicClient, ClientAccount, ClientAccountDTO>
	{
		private ClientOfferService ClientOfferService { get; }

		public ClientService(
			UserEntityRepository<Client, ClientAccount> repo,
			CrudService<User, PrivateClient> userService,
			ClientAccountService accountService,
			CommercialLinkService clService,
			PurchaseService purchaseService,
			Credentials credentials,
			ClientOfferService clientOfferService
		) : base(repo, userService, accountService, clService, purchaseService, credentials)
		{
			ClientOfferService = clientOfferService;
			NotRequiredForSaving = new string[] { "AdminPassword" };
			NotRequiredForUpdating = new string[] { "AdminPassword" };
		}

		public override void Delete(int id)
		{
			CheckCredentials(id);
			Client client = FindEntity(id);
			ClService.NullifyClient(id);
			ClientOfferService.NullifyClient(id);
			Repo.Delete(id);
			UserService.Delete(client.Id);
			AccountService.Delete(client.AccountId);
		}
	}
}
