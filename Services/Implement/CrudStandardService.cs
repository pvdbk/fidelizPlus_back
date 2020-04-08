using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace fidelizPlus_back.Services
{
    using AppModel;
    using Repositories;

    public class CrudStandardService<TEntity, TDTO> : CrudService<TEntity, TDTO>
        where TEntity : Entity, new()
        where TDTO : new()
    {
        public CrudRepository<TEntity> Repo { get; }
        public Utils Utils { get; }
        public FiltersHandler FiltersHandler { get; }
        public string[] UnexpectedForSaving { get; set; }
        public string[] NotRequiredForSaving { get; set; }
        public string[] UnexpectedForUpdating { get; set; }
        public string[] NotRequiredForUpdating { get; set; }

        public CrudStandardService(
            CrudRepository<TEntity> repo,
            Utils utils,
            FiltersHandler filtersHandler
        )
        {
            Repo = repo;
            Utils = utils;
            FiltersHandler = filtersHandler;
            UnexpectedForSaving = new string[0];
            NotRequiredForSaving = new string[0];
            UnexpectedForUpdating = new string[0];
            NotRequiredForUpdating = new string[0];
        }

        public TEntity FindEntity(int id)
        {
            TEntity entity = Repo.FindById(id);
            if (entity == default(TEntity))
            {
                throw new AppException($"{typeof(TEntity).Name} not found", 404);
            }
            return entity;
        }

        public IEnumerable<TDTO> FilterOrFindAll(string filter)
        {
            IEnumerable<TEntity> entities = Repo.FindAll().ToList();
            IEnumerable<TDTO> ret = entities.Select(EntityToDTO);
            if (filter != null)
            {
                ret = FiltersHandler.Apply(ret, new Tree(filter));
            }
            return entities.Select(EntityToDTO);
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
                    unexpected.Add(prop.Name);
                }
                else if (!notRequiredProps.Contains(prop.Name))
                {
                    missing.Add(prop.Name);
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

        public TEntity QuickSave(TDTO dto)
        {
            TEntity entity = DTOToEntity(dto);
            Repo.Save(entity);
            return entity;
        }

        public virtual (TDTO, int) Save(TDTO dto)
        {
            CheckDTOForSaving(dto);
            TEntity entity = QuickSave(dto);
            return (EntityToDTO(entity), entity.Id);
        }

        public void CheckDTOForUpdating(TDTO dto)
        {
            CheckDTO(dto, UnexpectedForUpdating, NotRequiredForUpdating);
        }

        public TEntity QuickUpdate(int id, TDTO dto)
        {
            TEntity entity = DTOToEntity(dto);
            entity.Id = id;
            return Repo.Update(entity);
        }

        public virtual TDTO Update(int id, TDTO dto)
        {
            CheckDTOForUpdating(dto);
            FindEntity(id);
            return EntityToDTO(QuickUpdate(id, dto));
        }
    }
}
