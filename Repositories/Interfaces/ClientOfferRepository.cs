namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public interface ClientOfferRepository : CrudRepository<ClientOffer>
    {
        public int NullifyClient(int clientId);
    }
}
