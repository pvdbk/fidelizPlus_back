using System.Collections.Generic;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface Repository<T> where T : Entity
    {
        public IEnumerable<T> Filter(Dictionary<string, object> schema);

        public IEnumerable<T> FindAll();

        public T FindById(int id);

        public T Save(T entity);

        public T Update(T entity);

        public bool Delete(int id);
    }
}
