namespace fidelizPlus_back.Services
{
	using AppDomain;
	using Repositories;

	public class ClientAccountService : AccountService<ClientAccount, ClientAccountDTO>
	{
		public ClientAccountService(ClientAccountRepository repo) : base(repo)
		{ }
	}
}
