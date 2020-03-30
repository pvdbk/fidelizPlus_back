using System.Collections.Generic;

namespace fidelizPlus_back.Repositories
{
    public interface Repository<T>
    {
        public IEnumerable<T> FindAll();

        public T FindById(int id);

        public T Save(T entity);

        public T Update(T entity);

        public bool Delete(int id);
    }
}
