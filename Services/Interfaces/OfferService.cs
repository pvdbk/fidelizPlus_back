namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;

    public interface OfferService : CrudService<Offer, OfferDTO>
    {
        public void NullifyTrader(int traderId);
    }
}
