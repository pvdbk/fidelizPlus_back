using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public interface CrudRepository<T> where T : Entity
    {
        public IQueryable<T> FindAll();

        public T FindById(int id);

        public void Save(T entity);

        public T Update(T entity);

        public void Delete(int id);

        public Func<object, EntityEntry> Entry { get; }
    }
}
