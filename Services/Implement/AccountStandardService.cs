namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;
    using Repositories;

    public class AccountStandardService<TEntity, TDTO> : CrudStandardService<TEntity, TDTO>
        where TEntity : Account, new()
        where TDTO : AccountDTO, new()
    {
        public AccountStandardService(
            CrudRepository<TEntity> repo,
            Utils utils,
            FiltersHandler filtersHandler
        ) : base(repo, utils, filtersHandler)
        {
            UnexpectedForSaving = new string[] { "Balance" };
            UnexpectedForUpdating = new string[] { "Balance" };
        }

        public override TDTO EntityToDTO(TEntity entity)
        {
            TDTO ret = base.EntityToDTO(entity);
            ret.Balance = entity.Balance;
            return ret;
        }

        public override TEntity DTOToEntity(TDTO dto)
        {
            TEntity account = base.DTOToEntity(dto);
            account.Balance = (dto.Balance == null) ? 0 : (decimal)dto.Balance;
            return account;
        }

        public override TDTO Update(int id, TDTO dto)
        {
            CheckDTOForUpdating(dto);
            dto.Balance = FindEntity(id).Balance;
            return EntityToDTO(QuickUpdate(id, dto));
        }
    }
}
