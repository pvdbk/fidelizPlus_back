using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public class UserEntityStandardRepository<TEntity, TAccount> : CrudStandardRepository<TEntity>, UserEntityRepository<TEntity> where TEntity : UserEntity<TAccount>
    {
        public UserEntityStandardRepository(AppContext ctxt, Utils utils) : base(ctxt, utils)
        { }

        public override IQueryable<TEntity> FindAll()
        {
            return base.FindAll().Include(entity => entity.User).Include(entity => entity.Account);
        }

        public void SeekReferences(TEntity entity)
        {
            Entry(entity).Reference("User").Load();
            Entry(entity).Reference("Account").Load();
        }
    }
}
