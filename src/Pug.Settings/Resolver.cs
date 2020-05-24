using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	public class Resolver
	{
		private readonly ISettingsSchema _settingsSchema;
		private readonly ISettingStore _settingStore;

		public Resolver(ISettingsSchema settingsSchema,
						ISettingStore settingStore)
		{
			_settingsSchema = settingsSchema;
			_settingStore = settingStore;
		}

		public Setting ResolveSetting(EntityIdentifier entity, string purpose, string name)
		{
			#region input validation
			
			if(entity == null) throw new ArgumentNullException(nameof(entity));
			
			if( string.IsNullOrWhiteSpace(entity.Type))
				throw new ArgumentException("Value cannot be null or whitespace.", "entity.Type");
			
			if( string.IsNullOrWhiteSpace(entity.Identifier))
				throw new ArgumentException("Value cannot be null or whitespace.", "entity.Identifier");
			
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			#endregion
			
			#region input clean-up
			
			entity = new EntityIdentifier()
			{
				Type = entity.Type.Trim(),
				Identifier = entity.Identifier.Trim()
			};
			
			if(string.IsNullOrWhiteSpace(purpose))
				purpose = string.Empty;
			else
				purpose = purpose.Trim();

			name = name.Trim();
			
			#endregion
			
			IEntityType entityType = _settingsSchema.GetEntityType(entity.Type);

			/*
				An exception should be thrown if entity-type does not exist, e.g. incorrect or has been removed from schema,
				to alert developer of usage of removed/unknown entity-type
			 */
			if(entityType == null)
				throw new UnknownEntityType();

			IEnumerable<SettingDefinition> settingDefinitions = entityType.GetSettings(purpose, name);

			SettingDefinition settingDefinition = settingDefinitions.FirstOrDefault();

			/*
				An exception should be thrown if setting definition does not exist, e.g. incorrect or has been removed from schema,
				to alert developer of usage of removed/unknown setting
			 */
			if(settingDefinition == null)
				throw new UnknownSetting();

			Setting setting = _settingStore.GetSetting(entity, purpose, name);

			if(setting != null)
			{
				// override stored setting default value with 'current' setting definition default value
				if(setting.ValueSource.Type == SettingValueSourceType.Default)
					if(settingDefinition.HasDefaultValue)
						setting.Value = settingDefinition.DefaultValue;

				return setting;
			}

			// if no stored setting is found, return setting based on 'default' specified in SettingDefinition
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

			// no stored setting found and setting definition has no 'default' value
			return null;
		}
	}
}