using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class UserEntityRepository<TEntity, TAccount> : CrudRepository<TEntity> where TEntity : UserEntity<TAccount>
    {
        public UserEntityRepository(AppContext ctxt) : base(ctxt)
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
