using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class CrudStandardRepository<T> : CrudRepository<T> where T : Entity
    {
        public Func<int> SaveChanges { get; }
        public DbSet<T> Entities { get; }
        public Func<object, EntityEntry> Entry { get; }
        public FiltersHandler FiltersHandler { get; }
        public Utils Utils { get; set; }

        private readonly IEnumerable<Type> writableTypes = new Type[] {
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(decimal),
            typeof(decimal?),
            typeof(string)
        };

        public CrudStandardRepository(Context context, FiltersHandler filtersHandler, Utils utils)
        {
            this.SaveChanges = context.SaveChanges;
            this.Entities = context.Set<T>();
            this.Entry = context.Entry;
            this.FiltersHandler = filtersHandler;
            this.Utils = utils;
        }

        public T FindById(int id)
        {
            return this.Entities.FirstOrDefault(entity => entity.Id == id);
        }

        public virtual IQueryable<T> FindAll()
        {
            return this.Entities;
        }

        public virtual IEnumerable<T> Filter(Tree filtersTree)
        {
            return FiltersHandler.Apply<T>(this.FindAll(), filtersTree);
        }

        public virtual void Save(T entity)
        {
            this.Entities.Add(entity);
            this.SaveChanges();
        }

        public bool Delete(int id)
        {
            T toDelete = this.FindById(id);
            bool found = toDelete != null;
            if (found)
            {
                this.Entities.Remove(toDelete);
                this.SaveChanges();
            }
            return found;
        }

        public T Update(T newEntity)
        {
            T toUpdate = this.FindById(newEntity.Id);
            if (toUpdate == null)
            {
                throw new AppException("You try to update something which not exists !", 500);
            }
            if (toUpdate != null)
            {
                IEnumerable<PropertyInfo> props = this.Utils.GetProps<T>().Where(prop => writableTypes.Contains(prop.PropertyType));
                foreach (PropertyInfo prop in props)
                {
                    prop.SetValue(toUpdate, prop.GetValue(newEntity));
                }
                this.SaveChanges();
            }
            return toUpdate;
        }
    }
}
