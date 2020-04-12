using System;

namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class RelatedToBothService<TEntity, TDTO> : CrudService<TEntity, TDTO>
        where TEntity : RelatedToBoth, new()
        where TDTO : RelatedToBothDTO, new()
    {
        public Action<TEntity> SeekReferences { get; }

        public RelatedToBothService(
            RelatedToBothRepository<TEntity> repo,
            Utils utils,
            FiltersHandler filtersHandler
        ) : base(repo, utils, filtersHandler)
        {
            SeekReferences = repo.SeekReferences;
        }

        public override TDTO EntityToDTO(TEntity entity)
        {
            TDTO ret = base.EntityToDTO(entity);
            SeekReferences(entity);
            CommercialLink cl = entity.CommercialLink;
            ret.ClientId = cl.ClientId;
            ret.TraderId = cl.TraderId;
            return ret;
        }
    }
}
