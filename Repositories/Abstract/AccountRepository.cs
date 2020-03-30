namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface AccountRepository : Repository<Account>
    {
        public int DeleteClient(int clientId);
    }
}
