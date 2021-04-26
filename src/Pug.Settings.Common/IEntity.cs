using System;

namespace Tensible
{
	public interface IEntity<TEntity>
		where TEntity : class
	{
		string Name { get; }
		
		IEntity<TEntity> With<TPurpose>(ISettings<TEntity, TPurpose> settings) where TPurpose : class;

		IEntity<TEntity> With<TSettings>(Func<TEntity, IServiceProvider, TSettings> settingsAccessor,
										Action<ISettings<TEntity, TSettings>> configurator)
			where TSettings : class;

		IEntity<TEntity> With<TSettings>(string name,
										Func<TEntity, IServiceProvider, TSettings> settingsAccessor,
										Action<ISettings<TEntity, TSettings>> configurator)
			where TSettings : class;
	}
}