namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public interface UserEntityRepository<T> : CrudRepository<T> where T : Entity
    {
        public void SeekReferences(T entity);
    }
}
