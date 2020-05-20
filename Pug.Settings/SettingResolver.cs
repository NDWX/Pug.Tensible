using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	public class SettingResolver
	{
		private readonly IEntityRelationshipResolver _entityRelationshipResolver;
		private readonly ISettingsSchema _settingsSchema;
		private readonly ISettingStore _settingStore;

		public SettingResolver(ISettingsSchema settingsSchema, IEntityRelationshipResolver entityRelationshipResolver,
								ISettingStore settingStore)
		{
			_settingsSchema = settingsSchema;
			_entityRelationshipResolver = entityRelationshipResolver;
			_settingStore = settingStore;
		}

		public Setting ResolveSetting(EntityIdentifier entity, string purpose, string name)
		{
			IEntityType entityType = _settingsSchema.GetEntityType(entity.Type);

			if(entityType == null)
				throw new UnknownEntityType();

			IEnumerable<SettingDefinition> settingDefinitions = entityType.GetSettings(purpose, name);

			SettingDefinition settingDefinition = settingDefinitions.FirstOrDefault();

			if(settingDefinition == null)
				throw new UnknownSetting();

			Setting setting = _settingStore.GetSetting(entity, purpose, name);

			if(setting != null)
				return setting;

			if(settingDefinition.HasDefaultValue)
				return new Setting
				{
					Purpose = purpose,
					Name = name,
					Value = settingDefinition.DefaultValue,
					ValueSource = new SettingValueSource
					{
						Type = SettingValueSourceType.Default
					}
				};

			return null;
		}
	}
}