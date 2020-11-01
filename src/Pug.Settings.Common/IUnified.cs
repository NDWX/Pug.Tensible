namespace Tensible
{
	public interface IUnified
	{
		TPurpose GetEffectiveSettings<TEntity, TPurpose>(string entityType, string purpose, TEntity entity)
			where TPurpose : class;

		TPurpose GetSettings<TEntity, TPurpose>(string entityType, string purpose, TEntity entity)
			where TPurpose : class;

		TPurpose GetSettings<TEntity, TPurpose>(TEntity entity) where TPurpose : class;
		TPurpose GetSettings<TEntity, TPurpose>(string purpose, TEntity entity) where TPurpose : class;

		TPurpose GetEffectiveSettings<TEntity, TPurpose>(TEntity entity) where TPurpose : class;

		TPurpose GetEffectiveSettings<TEntity, TPurpose>(string purpose, TEntity entity) where TPurpose : class;
	}
}