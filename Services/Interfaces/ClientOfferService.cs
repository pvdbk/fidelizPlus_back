namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;

    public interface ClientOfferService : CrudService<ClientOffer, ClientOfferDTO>
    {
        public void NullifyClient(int clientId);
    }
}
