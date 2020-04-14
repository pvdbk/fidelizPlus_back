using System;
using System.Collections.Generic;

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

        public RelatedToBothService(RelatedToBothRepository<TEntity> repo, Utils utils) : base(repo, utils)
        {
            SeekReferences = repo.SeekReferences;
        }

        public override IEnumerable<TEntity> GetEntities(Tree filterArg)
        {
            Tree entityTree = null;
            if (filterArg != null)
            {
                Tree dtoTree = Utils.ExtractTree<TDTO>(filterArg);
                Tree clTree = Utils.ExtractTree<CommercialLink>(dtoTree);
                clTree.Remove("id");
                clTree.Name = "commercialLink";
                entityTree = Utils.ExtractTree<TEntity>(dtoTree);
                entityTree.Add(clTree);
            }
            return Repo.GetEntities(Utils.TreeToTest<TEntity>(entityTree));
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
