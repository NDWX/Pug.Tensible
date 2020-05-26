using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	/// <summary>
	/// Responsible for resolving effective settings based on schema and settings stored in ISettingStore
	/// </summary>
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
		
		private static void ValidateEntityIdentifier(EntityIdentifier entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			if(string.IsNullOrWhiteSpace(entity.Type))
				throw new ArgumentException("Value cannot be null or whitespace.", "entity.Type");

			if(string.IsNullOrWhiteSpace(entity.Identifier))
				throw new ArgumentException("Value cannot be null or whitespace.", "entity.Identifier");
		}
		
		private static EntityIdentifier Normalize(EntityIdentifier entity)
		{
			entity = new EntityIdentifier()
			{
				Type = entity.Type.Trim(),
				Identifier = entity.Identifier.Trim()
			};
			return entity;
		}

		private static string NormalizePurposeName(string purpose)
		{
			if(string.IsNullOrWhiteSpace(purpose))
				purpose = string.Empty;
			else
				purpose = purpose.Trim();
			return purpose;
		}

		private static Setting ResolveSetting(string purpose, string name, Setting setting, SettingDefinition settingDefinition)
		{
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
					Name = settingDefinition.Name,
					Value = settingDefinition.DefaultValue,
					ValueSource = new SettingValueSource
					{
						Type = SettingValueSourceType.Default
					}
				};

			// no stored setting found and setting definition has no 'default' value
			return null;
		}
		
		/// <summary>
		/// Resolve all effective settings within a purpose for the specified entity.
		/// </summary>
		/// <param name="entity">Entity to for which settings are to be resolved</param>
		/// <param name="purpose">The purpose of the settings which are to be resolved</param>
		/// <returns>All effective settings for the specified entity and purpose</returns>
		/// <exception cref="UnknownEntityType">When specified entity type is not known within settings schema</exception>
		public IDictionary<string, Setting> ResolveSettings(EntityIdentifier entity, string purpose)
		{
			#region input validation
			
			ValidateEntityIdentifier(entity);

			#endregion
			
			#region input clean-up
			
			entity = Normalize(entity);
			
			purpose = NormalizePurposeName(purpose);
			
			#endregion
			
			IEntityType entityType = _settingsSchema.GetEntityType(entity.Type);

			/*
				An exception should be thrown if entity-type does not exist, e.g. incorrect or has been removed from schema,
				to alert developer of usage of removed/unknown entity-type
			 */
			if(entityType == null)
				throw new UnknownEntityType();

			IEnumerable<SettingDefinition> settingDefinitions = entityType.GetSettings(purpose);

			IDictionary<string, Setting> storedSettings = _settingStore.GetSettings(entity, purpose).ToDictionary(x => x.Name);
			
			IDictionary<string, Setting> resolvedSettings = new Dictionary<string, Setting>(settingDefinitions.Count());

			Setting setting;
			
			foreach(SettingDefinition definition in settingDefinitions)
			{
				setting = null;
				
				if( storedSettings.ContainsKey(definition.Name) )
					setting = storedSettings[definition.Name];

				resolvedSettings.Add(definition.Name, ResolveSetting(purpose, definition.Name, setting, definition));
			}

			return resolvedSettings;
		}

		/// <summary>
		/// Resolve effective setting within the purpose for the specified entity
		/// </summary>
		/// <param name="entity">Entity to for which settings are to be resolved</param>
		/// <param name="purpose">The purpose of the settings which are to be resolved</param>
		/// <param name="name">Name of the setting for the specified entity and purpose</param>
		/// <returns>Effective setting for the specified entity, purpose, and name</returns>
		/// <exception cref="ArgumentException">One of the specified arguments fail validation</exception>
		/// <exception cref="UnknownEntityType">Specified entity type is not known within settings schema</exception>
		/// <exception cref="UnknownSetting">Specified setting name is not known within settings schema</exception>
		public Setting ResolveSetting(EntityIdentifier entity, string purpose, string name)
		{
			#region input validation
			
			ValidateEntityIdentifier(entity);
			
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			#endregion
			
			#region input clean-up
			
			entity = Normalize(entity);
			
			purpose = NormalizePurposeName(purpose);

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

			return ResolveSetting(purpose, name, setting, settingDefinition);
		}
	}
}