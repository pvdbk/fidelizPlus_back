namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface PurchaseCommentRepository<T> : CrudRepository<T> where T : Entity, PurchaseComment
    {
        public int DeleteCommercialLink(int clId);
    }
}
