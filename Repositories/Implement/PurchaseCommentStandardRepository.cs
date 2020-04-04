using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Models;

    public class PurchaseCommentStandardRepository<T> : CrudStandardRepository<T>, PurchaseCommentRepository<T> where T : Entity, PurchaseComment
    {
        public PurchaseCommentStandardRepository(Context ctxt) : base(ctxt)
        {
        }

        public int DeleteCommercialLink(int clId)
        {
            List<int> toDelete = this.Entities
                .Where(entity => entity.CommercialLinkId == clId)
                .Select(entity => entity.Id)
                .ToList();
            foreach (int entityId in toDelete)
            {
                this.Delete(entityId);
            }
            this.SaveChanges();
            return toDelete.Count;
        }
    }
}
