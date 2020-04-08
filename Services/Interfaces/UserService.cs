namespace fidelizPlus_back.Services
{
    using AppModel;

    public interface UserService<TEntity, TDTO, TExtendedDTO, TAccountDTO> : CrudService<TEntity, TDTO>
        where TEntity : Entity, new()
        where TDTO : new()
    {
        public TExtendedDTO ExtendDTO(TDTO dto, int id);

        public TAccountDTO GetAccount(int id);

        public void CollectCl(TEntity entity);
    }
}
