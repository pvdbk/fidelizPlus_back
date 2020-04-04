namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface ClientOfferRepository : CrudRepository<ClientOffer>
    {
        public int NullifyClient(int clientId);
    }
}
