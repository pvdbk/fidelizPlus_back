namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface UserEntityRepository<T> : Repository<T> where T : Entity
    {
        public void FillUserProp(T entity);
    }
}
