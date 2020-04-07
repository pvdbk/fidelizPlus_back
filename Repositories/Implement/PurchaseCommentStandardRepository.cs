﻿using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Repositories
{
    using Errors;
    using Models;

    public class PurchaseCommentStandardRepository<T> : CrudStandardRepository<T>, PurchaseCommentRepository<T> where T : Entity, PurchaseComment
    {
        public PurchaseCommentStandardRepository(
            Error error,
            AppContext ctxt,
            FiltersHandler filtersHandler,
            Utils utils
        ) : base(error, ctxt, filtersHandler, utils)
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
