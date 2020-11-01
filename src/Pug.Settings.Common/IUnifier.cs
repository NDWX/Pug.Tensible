using Pug.Effable;

namespace Tensible
{
	public interface IUnifier
	{
		IEntity<TEntity> AddEntity<TEntity>() where TEntity : class;
		
		IEntity<TEntity> AddEntity<TEntity>(string name) where TEntity : class;

		IUnified Unify();
	}
}