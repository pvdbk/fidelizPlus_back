namespace fidelizPlus_back.Services
{
	using AppDomain;
	using Repositories;

	public class TraderAccountService : AccountService<TraderAccount, TraderAccountDTO>
	{
		public TraderAccountService(TraderAccountRepository repo) : base(repo)
		{ }
	}
}
