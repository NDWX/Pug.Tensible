namespace Tensible
{
	public interface IEntity<TEntity>
		where TEntity : class
	{
		string Name { get; }
		
		IEntity<TEntity> With<TPurpose>(ISettings<TEntity, TPurpose> settings) where TPurpose : class;
	}
}