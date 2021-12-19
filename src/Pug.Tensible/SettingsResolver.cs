using System;
using System.Collections.Generic;

namespace Pug.Tensible
{
	internal class SettingsResolver<TEntity, TPurpose> : ISettingsResolver
		where TEntity : class
		where TPurpose : class
	{
		public ISettingsDefinition Definition { get; }
		private readonly Type settingsType = typeof(TPurpose);

		private readonly Func<TEntity, IServiceProvider, TPurpose> _settingsAccessor;
		private readonly IEnumerable<SettingsParent<TEntity, TPurpose>> _parentSettings;

		internal SettingsResolver(ISettingsDefinition definition, Func<TEntity, IServiceProvider, TPurpose> settingsAccessor,
						IEnumerable<SettingsParent<TEntity, TPurpose>> parentSettings)
		{
			Definition = definition;
			_settingsAccessor = settingsAccessor;
			_parentSettings = parentSettings;
		}
		
		private void ValidateSettingsRetrievalCriteria<TEntityType, TSettingsPurpose>(TEntityType entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			Type expectedEntityTYpe = typeof(TEntityType);
			Type expectedSettingsType = typeof(TSettingsPurpose);

			if(expectedSettingsType != settingsType)
				throw new SettingsException(
					$"Settings '{Definition.Name}' is of type '{settingsType.FullName}', but '{expectedSettingsType.FullName}' is expected.");
		}
		
		TSettingsPurpose ISettingsResolver.Get<TEntityType, TSettingsPurpose>(TEntityType entity, IServiceProvider serviceProvider)
		{
			ValidateSettingsRetrievalCriteria<TEntityType, TSettingsPurpose>(entity);

			return _settingsAccessor(entity as TEntity, serviceProvider) as TSettingsPurpose;
		}

		TSettingsPurpose ISettingsResolver.GetEffective<TEntityType, TSettingsPurpose>(TEntityType entity, IServiceProvider serviceProvider)
		{
			ValidateSettingsRetrievalCriteria<TEntityType, TSettingsPurpose>(entity);

			TPurpose settings = _settingsAccessor(entity as TEntity, serviceProvider);

			foreach(SettingsParent<TEntity, TPurpose> parent in _parentSettings)
			{
				settings = parent.MapTo(entity as TEntity, settings, serviceProvider);
			}

			return settings as TSettingsPurpose;
		}
	}
}