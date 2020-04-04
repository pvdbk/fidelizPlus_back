namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface UserEntityRepository<T> : CrudRepository<T> where T : Entity
    {
        public void FillUserProp(T entity);
    }
}
