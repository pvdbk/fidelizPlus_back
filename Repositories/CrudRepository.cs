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
        public Utils Utils { get; }
        public Func<object, EntityEntry> Entry { get; }

        public CrudRepository(AppContext ctxt, Utils utils)
        {
            SaveChanges = ctxt.SaveChanges;
            Entities = ctxt.Set<T>();
            Entry = ctxt.Entry;
            Utils = utils;
        }

        public T FindById(int id)
        {
            return Entities.FirstOrDefault(entity => entity.Id == id);
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
            Entities.Remove(FindById(id));
            SaveChanges();
        }

        private readonly IEnumerable<Type> writableTypes = new Type[] {
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(decimal),
            typeof(decimal?),
            typeof(string)
        };

        public T Update(T newEntity)
        {
            T toUpdate = FindById(newEntity.Id);
            if (toUpdate == null)
            {
                throw new AppException("You try to update something which not exists !");
            }
            IEnumerable<PropertyInfo> props = Utils.GetProps<T>().Where(prop => writableTypes.Contains(prop.PropertyType));
            foreach (PropertyInfo prop in props)
            {
                prop.SetValue(toUpdate, prop.GetValue(newEntity));
            }
            SaveChanges();
            return toUpdate;
        }
    }
}
