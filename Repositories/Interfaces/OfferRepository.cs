namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public interface OfferRepository : CrudRepository<Offer>
    {
        public int NullifyTrader(int traderId);
    }
}
