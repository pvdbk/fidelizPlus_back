namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface ClientOfferRepository : Repository<ClientOffer>
    {
        public int NullifyClient(int clientId);
    }
}
