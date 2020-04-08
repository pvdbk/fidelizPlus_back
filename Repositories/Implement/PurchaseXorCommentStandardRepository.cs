﻿using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using AppModel;

    public class PurchaseXorCommentStandardRepository<T> : CrudStandardRepository<T>, PurchaseXorCommentRepository<T> where T : PurchaseComment
    {
        public PurchaseXorCommentStandardRepository(AppContext ctxt, Utils utils) : base(ctxt, utils)
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
    }
}
