using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;

    public interface CrudService<TEntity, TDTO> where TEntity : Entity where TDTO : DTO
    {
        public TEntity FindEntity(int id);

        public IEnumerable<TDTO> FilterOrFindAll(string filter);

        public TDTO FindById(int id);

        public bool IsUnexpectedProp(string propName);

        public bool IsUnexpectedForSaving(string propName);

        public bool IsUnexpectedForUpdating(string propName);

        public bool IsRequiredProp(string propName);

        public bool IsRequiredForSaving(string propName);

        public bool IsRequiredForUpdating(string propName);

        public TEntity DTOToEntity(TDTO dto);

        public TDTO EntityToDTO(TEntity entity);

        public void Delete(int id);

        public TDTO Save(TDTO dto);

        public TDTO Update(int id, TDTO dto);
    }
}
