namespace fidelizPlus_back.Repositories
{
	using AppDomain;

	public class ClientAccountRepository : CrudRepository<ClientAccount>
	{
		public ClientAccountRepository(AppContext ctxt) : base(ctxt)
		{ }
	}
}
