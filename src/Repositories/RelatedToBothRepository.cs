namespace fidelizPlus_back.Repositories
{
	using AppDomain;

	public class RelatedToBothRepository<T> : CrudRepository<T> where T : RelatedToBoth
	{
		public RelatedToBothRepository(AppContext ctxt) : base(ctxt)
		{ }

		public void SeekReferences(T entity) => Entry(entity).Reference("CommercialLink").Load();

		public override T FindEntity(int? id)
		{
			T entity = base.FindEntity(id);
			SeekReferences(entity);
			return entity;
		}
	}
}
