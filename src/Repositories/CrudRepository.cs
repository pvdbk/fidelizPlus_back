using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class CrudRepository<T> where T : Entity
    {
        public Func<int> SaveChanges { get; }
        private DbSet<T> dbSet { get; }
        public Func<object, EntityEntry> Entry { get; }
        private IEnumerable<PropertyInfo> UpdatableProps { get; }

        public CrudRepository(AppContext ctxt)
        {
            SaveChanges = ctxt.SaveChanges;
            dbSet = ctxt.Set<T>();
            Entry = ctxt.Entry;
            UpdatableProps = typeof(T).GetAtomicProps();
        }

        public virtual IQueryable<T> Entities => dbSet;

        public virtual T FindEntity(int? id)
        {
            T entity = id == null ? null : Entities.FirstOrDefault(entity => entity.Id == id);
            return entity != null
                ? entity
                : new AppException($"{typeof(T).Name} not found").Throw<T>();
        }

        public virtual void Save(T entity)
        {
            dbSet.Add(entity);
            SaveChanges();
        }

        public void Delete(int id)
        {
            dbSet.Remove(FindEntity(id));
            SaveChanges();
        }

        public T Update(T newEntity)
        {
            T toUpdate = FindEntity(newEntity.Id);
            if (toUpdate != newEntity)
            {
                UpdatableProps.ForEach(prop => prop.SetValue(toUpdate, prop.GetValue(newEntity)));
            }
            SaveChanges();
            return toUpdate;
        }
    }
}
