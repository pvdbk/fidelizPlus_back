namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;

    public interface CommercialLinkService : CrudService<CommercialLink, CommercialLink>
    {
        public void SeekReferences(CommercialLink cl);

        public void NullifyClient(int clientId);

        public void NullifyTrader(int traderId);

        public CommercialLinkStatus GetClStatus(CommercialLink cl);

        public CommercialLink FindWithBoth(int clientId, int traderId);
    }
}
