using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class UserEntityStandardRepository<T> : StandardRepository<T>, UserEntityRepository<T> where T : Entity, UserEntity, new()
    {
        public UserEntityStandardRepository(Context ctxt) : base(ctxt)
        {
        }

        public override IQueryable<T> FindAll()
        {
            return base.FindAll().Include(entity => entity.User);
        }

        public void FillUserProp(T entity)
        {
            this.Entry(entity).Reference("User").Load();
        }

        public override IEnumerable<T> Filter(Tree filtersTree)
        {
            IEnumerable<T> clients = base.Filter(filtersTree);
            return Utils.ApplyFilter<T, User>(clients, filtersTree, entity => entity.User, new string[] { "Id" });
        }
    }
}
