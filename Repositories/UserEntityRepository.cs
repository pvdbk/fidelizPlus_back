using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class UserEntityRepository<TEntity, TAccount> : CrudRepository<TEntity> where TEntity : UserEntity<TAccount>
    {
        public UserEntityRepository(AppContext ctxt, Utils utils) : base(ctxt, utils)
        { }

        public override IQueryable<TEntity> Everyone()
        {
            return Entities.Include(e => e.User).Include(e => e.Account);
        }

        public void SeekReferences(TEntity entity)
        {
            Entry(entity).Reference("User").Load();
            Entry(entity).Reference("Account").Load();
        }
    }
}
