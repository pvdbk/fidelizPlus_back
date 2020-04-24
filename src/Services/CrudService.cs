using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using Repositories;

    public class CrudService<TEntity, TDTO>
        where TEntity : Entity, new()
        where TDTO : class, new()
    {
        public CrudRepository<TEntity> Repo { get; }
        public string[] UnexpectedForSaving { get; set; }
        public string[] NotRequiredForSaving { get; set; }
        public string[] UnexpectedForUpdating { get; set; }
        public string[] NotRequiredForUpdating { get; set; }

        public CrudService(CrudRepository<TEntity> repo)
        {
            Repo = repo;
            UnexpectedForSaving = new string[0];
            NotRequiredForSaving = new string[0];
            UnexpectedForUpdating = new string[0];
            NotRequiredForUpdating = new string[0];
        }

        public IQueryable<TEntity> Entities => Repo.Entities;

        public TEntity FindEntity(int? id)
        {
            try
            {
                return Repo.FindEntity(id);
            }
            catch (AppException e)
            {
                throw new AppException(e.Content, 404);
            }
        }

        public virtual TEntity DTOToEntity(TDTO dto) => dto.CastAs<TEntity>();

        public virtual TDTO EntityToDTO(TEntity entity) => entity.CastAs<TDTO>();

        public virtual void Delete(int id) => Repo.Delete(id);

        public void CheckDTO(TDTO dto, string[] unexpectedProps, string[] notRequiredProps)
        {
            IEnumerable<PropertyInfo> propsToLook = typeof(TDTO).GetProps().Where(prop =>
                prop.GetValue(dto) == null ^
                unexpectedProps.Contains(prop.Name)
            );
            var unexpected = new List<string>();
            var missing = new List<string>();
            foreach (PropertyInfo prop in propsToLook)
            {
                if (prop.GetValue(dto) != null)
                {
                    unexpected.Add(prop.Name.FirstToLower());
                }
                else if (!notRequiredProps.Contains(prop.Name))
                {
                    missing.Add(prop.Name.FirstToLower());
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

        public void CheckDTOForSaving(TDTO dto) => CheckDTO(dto, UnexpectedForSaving, NotRequiredForSaving);

        public TEntity Save(TEntity entity)
        {
            Repo.Save(entity);
            return entity;
        }

        public TEntity QuickSave(TDTO dto) => Save(DTOToEntity(dto));

        public void CheckDTOForUpdating(TDTO dto) => CheckDTO(dto, UnexpectedForUpdating, NotRequiredForUpdating);

        public TEntity Update(TEntity entity) => Repo.Update(entity);

        public TEntity QuickUpdate(int id, TDTO dto)
        {
            TEntity entity = DTOToEntity(dto);
            entity.Id = id;
            return Update(entity);
        }

        public virtual TDTO CheckUpdate(int id, TDTO dto, HttpContext ctxt)
        {
            CheckDTOForUpdating(dto);
            return EntityToDTO(QuickUpdate(id, dto));
        }
    }
}
