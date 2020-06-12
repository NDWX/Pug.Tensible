using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Settings.Schema
{
	/// <summary>
	/// Responsible for resolving effective settings based on schema and settings stored in ISettingStore
	/// </summary>
	class Resolver : IResolver
	{
		private readonly ISettingsSchema _settingsSchema;
		private readonly ISettingStore _settingStore;
		private readonly IEntityRelationshipResolver _entityRelationshipResolver;

		public Resolver(ISettingsSchema settingsSchema,
						ISettingStore settingStore, IEntityRelationshipResolver entityRelationshipResolver)
		{
			_settingsSchema = settingsSchema;
			_settingStore = settingStore;
			_entityRelationshipResolver = entityRelationshipResolver;
		}
		
		[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
		private static void ValidateEntityIdentifier(EntityIdentifier entity)
		{
			if(entity == null) throw new ArgumentNullException(nameof(entity));

			if(string.IsNullOrWhiteSpace(entity.Type))
				// ReSharper disable once NotResolvedInText
				throw new ArgumentException("Value cannot be null or whitespace.", "entity.Type");

			if(string.IsNullOrWhiteSpace(entity.Identifier))
				// ReSharper disable once NotResolvedInText
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
		
		private Setting resolveSetting(EntityIdentifier entity, string purpose, string name)
		{
			EntityTypeSchema entityType = _settingsSchema.GetEntityType(entity.Type) as EntityTypeSchema;

			/*
				An exception should be thrown if entity-type does not exist, e.g. incorrect or has been removed from schema,
				to alert developer of usage of removed/unknown entity-type
			 */
			if(entityType == null)
				throw new UnknownEntityType();

			IEnumerable<ISettingSchema> settingSchemas = entityType.GetSettings(purpose, name);

			ISettingSchema settingSchema = settingSchemas.FirstOrDefault();

			/*
				An exception should be thrown if setting definition does not exist, e.g. incorrect or has been removed from schema,
				to alert developer of usage of removed/unknown setting
			 */
			if(settingSchema == null)
				throw new UnknownSetting();

			Setting setting = _settingStore.GetSetting(entity, purpose, name);

			return resolveSetting(entityType, purpose, settingSchema, setting);
		}

		private Setting resolveSetting(EntityTypeSchema entityType, string purpose, ISettingSchema schema,
											Setting setting)
		{
			SettingDefinition settingDefinition = schema.Definition;
			
			if(setting != null && setting.ValueSource.Type == SettingValueSourceType.User)
			{
				setting.ValueSource = entityType.UserValueSource;

				return setting;
			}

			// if no stored setting is found, return setting based on 'default' specified in SettingDefinition
			if(schema.Source.Type == DefinitionSourceType.EntityType)
			{
				if(settingDefinition.HasDefaultValue)
				{
					return new Setting
					{
						Purpose = purpose,
						Name = settingDefinition.Name,
						Value = settingDefinition.DefaultValue,
						ValueSource = entityType.DefaultValueSource
					};
				}
			}
			else // if setting is inherited from parent
			{
				IEntityPurposeSchema purposeSchema = entityType.GetPurpose(purpose);

				string parentEntityIdentifier =
					_entityRelationshipResolver.GetEntityParent(new EntityIdentifier(),
																purposeSchema.Definition.ParentEntityType);

				EntityIdentifier parentEntity = new EntityIdentifier()
					{Type = purposeSchema.Definition.ParentEntityType, Identifier = parentEntityIdentifier};

				Setting parentSetting = resolveSetting(parentEntity, purpose, settingDefinition.Name);

				if(parentSetting == null)
					return null;

				SettingValueSourceType valueSourceType = SettingValueSourceType.Parent;

				if((parentSetting.ValueSource.Type & SettingValueSourceType.Default) == SettingValueSourceType.Default)
					valueSourceType = valueSourceType | SettingValueSourceType.Default;
				else if((parentSetting.ValueSource.Type & SettingValueSourceType.User) == SettingValueSourceType.User)
					valueSourceType = valueSourceType | SettingValueSourceType.User;

				return new Setting()
				{
					Name = settingDefinition.Name,
					Purpose = purpose,
					Value = parentSetting.Value,
					ValueSource = new SettingValueSource(
							valueSourceType, 
							parentSetting.ValueSource.EntityType,
							(parentSetting.ValueSource.Type & SettingValueSourceType.Parent) == SettingValueSourceType.Parent?
								parentSetting.ValueSource.Source : null
					)
				};
			}

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
			
			EntityTypeSchema entityType = _settingsSchema.GetEntityType(entity.Type) as EntityTypeSchema;

			/*
				An exception should be thrown if entity-type does not exist, e.g. incorrect or has been removed from schema,
				to alert developer of usage of removed/unknown entity-type
			 */
			if(entityType == null)
				throw new UnknownEntityType();

			IEnumerable<ISettingSchema> settingSchemas = entityType.GetSettings(purpose);

			IDictionary<string, Setting> storedSettings = _settingStore.GetSettings(entity, purpose).ToDictionary(x => x.Name);
			
			IDictionary<string, Setting> resolvedSettings = new Dictionary<string, Setting>(settingSchemas.Count());

			Setting setting;
			
			foreach(ISettingSchema schema in settingSchemas)
			{
				setting = null;
				
				if( storedSettings.ContainsKey(schema.Definition.Name) )
					setting = storedSettings[schema.Definition.Name];

				resolvedSettings.Add(schema.Definition.Name, resolveSetting(entityType, purpose, schema, setting));
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
			
			return resolveSetting(entity, purpose, name);
		}
	}
}