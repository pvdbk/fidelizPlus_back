using System;
using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using Errors;
    using Models;
    using Repositories;

    public class CrudStandardService<TEntity, TDTO> : CrudService<TEntity, TDTO> where TEntity : Entity, new() where TDTO : DTO.DTO, new()
    {
        public Error Error { get; }
        public CrudRepository<TEntity> Repo { get; }
        public Utils Utils { get; }

        public CrudStandardService(
            Error error,
            CrudRepository<TEntity> repo,
            Utils utils
        )
        {
            this.Error = error;
            this.Repo = repo;
            this.Utils = utils;
        }

        public TEntity FindEntity(int id)
        {
            TEntity entity = this.Repo.FindById(id);
            if (entity == default(TEntity))
            {
                this.Error.Throw($"{typeof(TEntity).Name} not found", 404);
            }
            return entity;
        }

        public IEnumerable<TDTO> FilterOrFindAll(string filter)
        {
            IEnumerable<TEntity> entities = filter == null ? this.Repo.FindAll().ToList() : this.Repo.Filter(new Tree(filter, this.Error));
            return entities.Select(this.EntityToDTO);
        }

        public TDTO FindById(int id)
        {
            return this.EntityToDTO(this.FindEntity(id));
        }

        public void CheckDTO(TDTO dto, Func<string, bool> IsUnexpectedProp, Func<string, bool> IsRequiredProp)
        {
            var errorsStrings = new List<string>();
            IEnumerable<string> problematicProps = this.Utils.GetProps<TDTO>()
                .Where(prop => IsUnexpectedProp(prop.Name) && prop.GetValue(dto) != null)
                .Select(prop => prop.Name);
            if (problematicProps.Count() != 0)
            {
                errorsStrings.Add(this.Utils.ListToMessage("Unexpected", problematicProps));
            }
            problematicProps = this.Utils.GetProps<TDTO>()
                .Where(prop => IsRequiredProp(prop.Name) && prop.GetValue(dto) == null)
                .Select(prop => prop.Name);
            ;
            if (problematicProps.Count() != 0)
            {
                errorsStrings.Add(this.Utils.ListToMessage("Missing", problematicProps));
            }
            if (errorsStrings.Count != 0)
            {
                this.Error.Throw(this.Utils.Join(errorsStrings, "\n"), 400);
            }
        }

        public virtual bool IsUnexpectedProp(string propName)
        {
            return propName == "Id";
        }

        public virtual bool IsUnexpectedForSaving(string propName)
        {
            return this.IsUnexpectedProp(propName);
        }

        public virtual bool IsUnexpectedForUpdating(string propName)
        {
            return this.IsUnexpectedProp(propName);
        }

        public virtual bool IsRequiredProp(string propName)
        {
            return propName != "Id";
        }

        public virtual bool IsRequiredForSaving(string propName)
        {
            return this.IsRequiredProp(propName);
        }

        public virtual bool IsRequiredForUpdating(string propName)
        {
            return this.IsRequiredProp(propName);
        }

        public virtual TEntity DTOToEntity(TDTO dto)
        {
            return this.Utils.Cast<TEntity, TDTO>(dto, dto.Id);
        }

        public virtual TDTO EntityToDTO(TEntity entity)
        {
            return this.Utils.Cast<TDTO, TEntity>(entity, entity.Id);
        }

        public virtual void Delete(int id)
        {
            this.Repo.Delete(id);
        }

        public virtual TDTO Save(TDTO dto)
        {
            CheckDTO(dto, this.IsUnexpectedForSaving, this.IsRequiredForSaving);
            TEntity entity = this.DTOToEntity(dto);
            this.Repo.Save(entity);
            return this.EntityToDTO(entity);
        }

        public virtual TDTO Update(int id, TDTO dto)
        {
            TEntity entity = this.FindEntity(id);
            CheckDTO(dto, this.IsUnexpectedForUpdating, this.IsRequiredForUpdating);
            dto.Id = id;
            entity = this.Repo.Update(this.DTOToEntity(dto));
            return this.EntityToDTO(entity);
        }
    }
}
