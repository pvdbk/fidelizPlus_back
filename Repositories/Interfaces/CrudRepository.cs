using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public interface CrudRepository<T> where T : Entity
    {
        public IEnumerable<T> Filter(Tree filtersTree);

        public IQueryable<T> FindAll();

        public T FindById(int id);

        public void Save(T entity);

        public T Update(T entity);

        public bool Delete(int id);

        public Func<object, EntityEntry> Entry { get; }
    }
}
