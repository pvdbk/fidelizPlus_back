namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public interface CommercialLinkRepository : CrudRepository<CommercialLink>
    {
        public int NullifyClient(int clientId);

        public int NullifyTrader(int traderId);

        public void SeekReferences(CommercialLink cl);
    }
}
