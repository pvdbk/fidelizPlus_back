namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface ClientAccountRepository : CrudRepository<ClientAccount>
    {
        public int DeleteClient(int clientId);
    }
}
