namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface TraderAccountRepository : CrudRepository<TraderAccount>
    {
        public int DeleteTrader(int traderId);
    }
}
