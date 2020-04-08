namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public interface PurchaseXorCommentRepository<T> : CrudRepository<T> where T : PurchaseComment
    {
        public int DeleteCommercialLink(int clId);
    }
}
