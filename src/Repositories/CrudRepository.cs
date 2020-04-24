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
        public DbSet<T> Entities { get; }
        public Func<object, EntityEntry> Entry { get; }
        public IEnumerable<PropertyInfo> UpdatableProps { get; }

        public CrudRepository(AppContext ctxt)
        {
            SaveChanges = ctxt.SaveChanges;
            Entities = ctxt.Set<T>();
            Entry = ctxt.Entry;
            UpdatableProps = typeof(T).GetAtomicProps();
        }

        public T FindEntity(int? id)
        {
            T entity = id == null ? null : Entities.FirstOrDefault(entity => entity.Id == id);
            if (entity == null)
            {
                throw new AppException($"{typeof(T).Name} not found");
            }
            return entity;
        }

        public virtual IQueryable<T> FindAll()
        {
            return Entities;
        }

        public virtual void Save(T entity)
        {
            Entities.Add(entity);
            SaveChanges();
        }

        public void Delete(int id)
        {
            Entities.Remove(FindEntity(id));
            SaveChanges();
        }

        public T Update(T newEntity)
        {
            T toUpdate = FindEntity(newEntity.Id);
            if (toUpdate != newEntity)
            {
                foreach (PropertyInfo prop in UpdatableProps)
                {
                    prop.SetValue(toUpdate, prop.GetValue(newEntity));
                }
            }
            SaveChanges();
            return toUpdate;
        }
    }
}
