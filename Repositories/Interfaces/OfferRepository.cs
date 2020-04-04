namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface OfferRepository : CrudRepository<Offer>
    {
        public int NullifyTrader(int traderId);
    }
}
