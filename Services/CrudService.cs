using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Repositories;

    public class CrudService<TEntity, TDTO>
        where TEntity : Entity, new()
        where TDTO : new()
    {
        public CrudRepository<TEntity> Repo { get; }
        public Utils Utils { get; }
        public string[] UnexpectedForSaving { get; set; }
        public string[] NotRequiredForSaving { get; set; }
        public string[] UnexpectedForUpdating { get; set; }
        public string[] NotRequiredForUpdating { get; set; }

        public CrudService(CrudRepository<TEntity> repo, Utils utils)
        {
            Repo = repo;
            Utils = utils;
            UnexpectedForSaving = new string[0];
            NotRequiredForSaving = new string[0];
            UnexpectedForUpdating = new string[0];
            NotRequiredForUpdating = new string[0];
        }

        public TEntity FindEntity(int? id)
        {
            return Repo.FindEntity(id);
        }

        public IEnumerable<TDTO> FilterOrFindAll(string filter)
        {
            IEnumerable<TEntity> entities = Repo.FindAll();
            return Utils.ApplyFilter(entities.Select(EntityToDTO), filter);
        }

        public TDTO FindById(int id)
        {
            return EntityToDTO(FindEntity(id));
        }

        public virtual TEntity DTOToEntity(TDTO dto)
        {
            return Utils.Cast<TEntity, TDTO>(dto);
        }

        public virtual TDTO EntityToDTO(TEntity entity)
        {
            return Utils.Cast<TDTO, TEntity>(entity);
        }

        public virtual void Delete(int id)
        {
            Repo.Delete(id);
        }

        public void CheckDTO(TDTO dto, string[] unexpectedProps, string[] notRequiredProps)
        {
            IEnumerable<PropertyInfo> propsToLook = Utils.GetProps<TDTO>().Where(prop =>
                prop.GetValue(dto) == null ^
                unexpectedProps.Contains(prop.Name)
            );
            var unexpected = new List<string>();
            var missing = new List<string>();
            foreach (PropertyInfo prop in propsToLook)
            {
                if (prop.GetValue(dto) != null)
                {
                    unexpected.Add(Utils.FirstToLower(prop.Name));
                }
                else if (!notRequiredProps.Contains(prop.Name))
                {
                    missing.Add(Utils.FirstToLower(prop.Name));
                }
            }
            int errorCode = 0;
            errorCode += unexpected.Count == 0 ? 0 : 1;
            errorCode += missing.Count == 0 ? 0 : 2;
            object error =
                errorCode == 0 ? (object)null :
                errorCode == 1 ? (object)new { unexpected } :
                errorCode == 2 ? (object)new { missing } :
                (object)new { unexpected, missing };
            if (error != null)
            {
                throw new AppException(error, 400);
            }
        }

        public void CheckDTOForSaving(TDTO dto)
        {
            CheckDTO(dto, UnexpectedForSaving, NotRequiredForSaving);
        }

        public TEntity Save(TEntity entity)
        {
            Repo.Save(entity);
            return entity;
        }

        public TEntity QuickSave(TDTO dto)
        {
            return Save(DTOToEntity(dto));
        }

        public virtual (TDTO, int) CheckSave(TDTO dto)
        {
            CheckDTOForSaving(dto);
            TEntity entity = QuickSave(dto);
            return (EntityToDTO(entity), entity.Id);
        }

        public void CheckDTOForUpdating(TDTO dto)
        {
            CheckDTO(dto, UnexpectedForUpdating, NotRequiredForUpdating);
        }

        public TEntity Update(TEntity entity)
        {
            return Repo.Update(entity);
        }

        public TEntity QuickUpdate(int id, TDTO dto)
        {
            TEntity entity = DTOToEntity(dto);
            entity.Id = id;
            return Update(entity);
        }

        public virtual TDTO CheckUpdate(int id, TDTO dto)
        {
            CheckDTOForUpdating(dto);
            FindEntity(id);
            return EntityToDTO(QuickUpdate(id, dto));
        }
    }
}
