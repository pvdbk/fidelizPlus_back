using System.Collections.Generic;
using System.Linq;
using System;

namespace fidelizPlus_back.Services
{
    using Models;
    using Repositories;

    public class CrudStandardService<TEntity, TDTO> : CrudService<TEntity, TDTO> where TEntity : Entity, new() where TDTO : DTO.DTO, new()
    {
        protected CrudRepository<TEntity> repo;

        public CrudStandardService(CrudRepository<TEntity> repo)
        {
            this.repo = repo;
        }

        public TEntity FindEntity(int id)
        {
            TEntity entity = this.repo.FindById(id);
            if (entity == default(TEntity))
            {
                throw new AppException($"{typeof(TEntity).Name} not found", 404);
            }
            return entity;
        }

        public IEnumerable<TDTO> FilterOrFindAll(string filter)
        {
            IEnumerable<TEntity> entities = filter == null ? this.repo.FindAll().ToList() : this.repo.Filter(new Tree(filter));
            return entities.Select(this.EntityToDTO);
        }

        public TDTO FindById(int id)
        {
            return this.EntityToDTO(this.FindEntity(id));
        }

        public static void CheckDTO(TDTO dto, Func<string, bool> IsUnexpectedProp, Func<string, bool> IsRequiredProp)
        {
            var errorsStrings = new List<string>();
            IEnumerable<string> problematicProps = Utils.GetProps<TDTO>()
                .Where(prop => IsUnexpectedProp(prop.Name) && prop.GetValue(dto) != null)
                .Select(prop => prop.Name);
            if (problematicProps.Count() != 0)
            {
                errorsStrings.Add(Utils.ListToMessage("Unexpected", problematicProps));
            }
            problematicProps = Utils.GetProps<TDTO>()
                .Where(prop => IsRequiredProp(prop.Name) && prop.GetValue(dto) == null)
                .Select(prop => prop.Name);
            ;
            if (problematicProps.Count() != 0)
            {
                errorsStrings.Add(Utils.ListToMessage("Missing", problematicProps));
            }
            if (errorsStrings.Count != 0)
            {
                throw new AppException(Utils.Join(errorsStrings, "\n"), 400);
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
            return Utils.Cast<TEntity, TDTO>(dto, dto.Id);
        }

        public virtual TDTO EntityToDTO(TEntity entity)
        {
            return Utils.Cast<TDTO, TEntity>(entity, entity.Id);
        }

        public virtual void Delete(int id)
        {
            this.repo.Delete(id);
        }

        public virtual TDTO Save(TDTO dto)
        {
            CheckDTO(dto, this.IsUnexpectedForSaving, this.IsRequiredForSaving);
            TEntity entity = this.DTOToEntity(dto);
            this.repo.Save(entity);
            return this.EntityToDTO(entity);
        }

        public virtual TDTO Update(int id, TDTO dto)
        {
            TEntity entity = this.FindEntity(id);
            CheckDTO(dto, this.IsUnexpectedForUpdating, this.IsRequiredForUpdating);
            dto.Id = id;
            entity = this.repo.Update(this.DTOToEntity(dto));
            return this.EntityToDTO(entity);
        }
    }
}
