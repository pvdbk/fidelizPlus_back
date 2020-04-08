using System.Collections.Generic;

namespace fidelizPlus_back.Services
{
    using AppModel;

    public interface CrudService<TEntity, TDTO>
        where TEntity : Entity, new()
        where TDTO : new()
    {
        public TEntity FindEntity(int id);

        public IEnumerable<TDTO> FilterOrFindAll(string filter);

        public TDTO FindById(int id);

        public void CheckDTOForSaving(TDTO dto);

        public void CheckDTOForUpdating(TDTO dto);

        public TEntity DTOToEntity(TDTO dto);

        public TDTO EntityToDTO(TEntity entity);

        public void Delete(int id);

        public (TDTO, int) Save(TDTO dto);

        public TDTO Update(int id, TDTO dto);

        public TEntity QuickSave(TDTO dto);

        public TEntity QuickUpdate(int id, TDTO dto);
    }
}
