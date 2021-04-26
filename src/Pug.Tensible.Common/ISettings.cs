using System;

namespace Pug.Tensible
{
	public interface ISettings<TEntity, TSettings> : ISettingsDefinition
		where TEntity : class 
		where TSettings : class
	{
		Func<TEntity, IServiceProvider, TSettings> SettingsAccessor { get; }

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap);

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		);

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		);

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class;
	}
}