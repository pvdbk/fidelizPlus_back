namespace fidelizPlus_back.Repositories
{
	using AppDomain;

	public class TraderAccountRepository : CrudRepository<TraderAccount>
	{
		public TraderAccountRepository(AppContext ctxt) : base(ctxt)
		{ }
	}
}
