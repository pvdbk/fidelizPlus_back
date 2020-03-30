using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class StandardRepository<T> : Repository<T> where T : Entity, new()
    {
        public Func<int> SaveChanges { get; }
        public DbSet<T> Entities { get; }
        public Func<object, EntityEntry> Entry { get; }

        public StandardRepository(Context context)
        {
            this.SaveChanges = context.SaveChanges;
            this.Entities = context.Set<T>();
            this.Entry = context.Entry;
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
            return Utils.ApplyFilter<T, T>(this.FindAll(), filtersTree);
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
            if (toUpdate != null)
            {
                IEnumerable<PropertyInfo> props = Utils.GetProps<T>();
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
