using System;

namespace Tensible
{
	public interface ISettings<TEntity, TPurpose> : ISettingsDefinition
		where TEntity : class 
		where TPurpose : class
	{
		Func<TEntity, IServiceProvider, TPurpose> SettingsAccessor { get; }

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap);

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		);

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		);

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class;

		ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class;
	}
}