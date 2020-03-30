namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface CommercialLinkRepository : Repository<CommercialLink>
    {
        public int NullifyClient(int clientId);

        public int NullifyTrader(int traderId);
    }
}
