using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppDomain;

    public class RelatedToBothRepository<T> : CrudRepository<T> where T : RelatedToBoth
    {
        public RelatedToBothRepository(AppContext ctxt) : base(ctxt)
        { }

        public int DeleteCommercialLink(int clId)
        {
            List<int> toDelete = Entities
                .Where(entity => entity.CommercialLinkId == clId)
                .Select(entity => entity.Id)
                .ToList();
            foreach (int entityId in toDelete)
            {
                Delete(entityId);
            }
            SaveChanges();
            return toDelete.Count;
        }

        public void SeekReferences(T entity)
        {
            Entry(entity).Reference("CommercialLink").Load();
        }
    }
}
