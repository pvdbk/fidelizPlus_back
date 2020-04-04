namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface CommercialLinkRepository : CrudRepository<CommercialLink>
    {
        public int NullifyClient(int clientId);

        public int NullifyTrader(int traderId);
    }
}
