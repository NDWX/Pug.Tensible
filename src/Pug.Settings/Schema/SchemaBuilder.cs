using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Settings.Schema
{
	/// <summary>
	/// Provide and implementation of <see cref="ISchemaBuilder">ISchemaBuilder</see> interface
	/// </summary>
	public class SchemaBuilder : ISchemaBuilder
	{
		private readonly Dictionary<string, EntityTypeSchema> entityTypes;
		private readonly Dictionary<string, PurposeInfo> purposes;

		public SchemaBuilder()
		{
			purposes = new Dictionary<string, PurposeInfo>();
			entityTypes = new Dictionary<string, EntityTypeSchema>();
		}

		#region ISchemaBuilder Members

		/// <summary>
		/// Register a setting purpose
		/// </summary>
		/// <param name="name">Name of the purpose</param>
		/// <param name="description">Description of purpose</param>
		/// <returns>The same instance of this class</returns>
		/// <exception cref="ArgumentException">Name or description is null or empty</exception>
		/// <exception cref="DuplicateNameException">Name of purpose has already been used by another purpose</exception>
		public ISchemaBuilder RegisterPurpose(string name, string description)
		{
			#region input validation
			
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			#endregion
			
			#region input clean-up
			
			name = name.Trim();
			
			if(description == null) 
				description = string.Empty;
			else 
				description = description.Trim();

			#endregion
			
			if(purposes.ContainsKey(name))
				throw new DuplicateNameException();

			purposes.Add(name, new PurposeInfo(name, description));

			return this;
		}

		private void ResolvePurposeSchema(EntityPurposeDefinition purposeDefinition, IDictionary<string, EntityPurposeSchema> entityTypePurposes,
										DefinitionSource selfDefinitionSource)
		{
			string purposeKey = purposeDefinition.Name;

			if(purposeKey != string.Empty && entityTypePurposes.ContainsKey(purposeKey))
				throw new DuplicateNameException(
					$"Duplicate purpose name '{purposeKey}' specified for entity type");

			// ensure purpose info exists
			if(!this.purposes.ContainsKey(purposeKey))
				throw new UnknownPurpose(purposeKey);

			// determine duplicated setting names in 'purpose'
			IEnumerable<IGrouping<string, SettingDefinition>> duplicatedNames =
				from settingDefinition in purposeDefinition.Settings
				group settingDefinition by settingDefinition.Name.Trim()
				into nameDefinitions
				where nameDefinitions.Count() > 1
				select nameDefinitions;

			if(duplicatedNames.Any())
				throw new DuplicateNameException(duplicatedNames.First().Key);

			#region inheritability determinations
			
			bool inheritable = false;

			List<string> inheritableSettings = null; // null indicates all settings are inheritable if purpose is so

			PurposeSettingsInheritance purposeInheritability = purposeDefinition.Inheritability;
			
			if(purposeInheritability.InheritanceType == PurposeSettingsInheritanceType.Inherit)
			{
				inheritable = true;

				if(purposeInheritability.ApplicableSettings?.Any() ?? false)
					inheritableSettings = new List<string>(purposeInheritability.ApplicableSettings);
			}
			else
			{
				if(purposeInheritability.InheritanceType == PurposeSettingsInheritanceType.DoNotInherit)
				{
					if(purposeInheritability.ApplicableSettings?.Any() ?? false)
					{
						IEnumerable<string> candidates = purposeDefinition
														.Settings.Select(x => x.Name.Trim())
														.Except(purposeInheritability.ApplicableSettings);

						if(candidates.Any())
						{
							inheritable = true;
							inheritableSettings = new List<string>(candidates);
						}
					}
				}
			}
			
			#endregion

			// check parent entity type and purpose inheritability

			EntityTypeSchema parentSchema = null;
			DefinitionSource parentDefinitionSource = null;
			
			if(!string.IsNullOrWhiteSpace(purposeDefinition.ParentEntityType))
			{
				// ensure parent entity type is known
				if(!entityTypes.ContainsKey(purposeDefinition.ParentEntityType))
					throw new UnknownEntityType(purposeDefinition.ParentEntityType);

				// get parent entity type schema
				parentSchema = entityTypes[purposeDefinition.ParentEntityType];

				IEntityPurposeSchema parentPurpose = parentSchema.GetPurpose(purposeKey);

				// check purpose exists in parent entity type
				if(parentPurpose == null)
					throw new UnknownPurpose(purposeKey,
											$"Parent entity type '{parentSchema.Info.Name}' does not have purpose '{purposeKey}'");

				// check parent entity type purpose is inheritable
				if(parentPurpose.Definition.PurposeSettingsInheritanceType == PurposeSettingsInheritanceType.DoNotInherit)
					throw new NotInheritable(
						"Purpose '{purposeDefinition.Name}' of entity type '{parentSchema.Info.Name}' is not inheritable");

				// define reusable setting 'parent' source
				parentDefinitionSource =
					new DefinitionSource(DefinitionSourceType.ParentEntityType, purposeDefinition.ParentEntityType);
			}

			#region resolve 'effective' SettingSchemas

			IDictionary<string, ISettingSchema> settingDefinitions =
				new Dictionary<string, ISettingSchema>(purposeDefinition.Settings?.Count() ?? 0);

			// include self defined settings
			foreach(SettingDefinition settingDefinition in purposeDefinition.Settings)
			{
				settingDefinitions.Add(
						settingDefinition.Name,
						new SettingSchema()
						{
							Source = selfDefinitionSource,
							Definition = settingDefinition,
							Inheritable = inheritable && (inheritableSettings?.Contains(settingDefinition.Name) ?? true)
						}
					);
			}

			// resolve inherited settings
			if(parentSchema != null)
			{
				PurposeSettingsInheritance parentSettingsInheritance = purposeDefinition.Inheritance;
				
				foreach(ISettingSchema settingSchema in parentSchema.GetSettings(purposeKey))
				{
					// ensure setting is inheritable
					if(!settingSchema.Inheritable)
						continue;

					SettingDefinition settingDefinition = settingSchema.Definition;
					
					// skip if setting is already defined by 'self' 
					if(settingDefinitions.ContainsKey(settingDefinition.Name))
						continue;
					
					IEnumerable<string> inheritanceApplicableSettings = parentSettingsInheritance.ApplicableSettings;
					
					// evaluate purpose definition inheritance
					if(parentSettingsInheritance.InheritanceType == PurposeSettingsInheritanceType.Inherit)
					{ // check for explicit inclusion if list is not empty
						if(inheritanceApplicableSettings != null &&
							!inheritanceApplicableSettings.Contains(settingDefinition.Name))
							continue;
					}
					else if(parentSettingsInheritance.InheritanceType == PurposeSettingsInheritanceType.DoNotInherit)
					{ // check for explicit exclusion if list is not empty
						if(inheritanceApplicableSettings != null && inheritanceApplicableSettings.Contains(settingDefinition.Name) )
							continue;
					}

					settingDefinitions.Add(
							settingDefinition.Name,
							new SettingSchema()
							{
								// determine whether source is direct parent or inherited by parent
								Source = settingSchema.Source.Type == DefinitionSourceType.EntityType ?
											parentDefinitionSource : settingSchema.Source,
								Definition = settingDefinition,
								Inheritable = inheritable &&
											(inheritableSettings?.Contains(settingDefinition.Name) ?? true)
							}
						);
				}
			}

			// todo: Warn about unknown 'notInheritableSettings' entry

			#endregion

			entityTypePurposes.Add(
					purposeKey,
					new EntityPurposeSchema()
					{
						Definition = purposeDefinition,
						Settings = settingDefinitions,
						Source = selfDefinitionSource
					}
				);
		}

		/// <summary>
		/// Register an entity type with all its purposes and settings
		/// </summary>
		/// <param name="name">Name of entity type</param>
		/// <param name="description">Description of entity type</param>
		/// <param name="purposeSettings">Entity purposes and settings</param>
		/// <returns>The same instance of this class</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="DuplicateNameException">Entity, or purpose within entity or setting within a purpose is duplicated</exception>
		/// <exception cref="UnknownPurpose">Purpose has not been pre-registered using <see cref="RegisterPurpose">RegisterPurpose</see></exception>
		public ISchemaBuilder RegisterEntityType(string name, string description,
												IEnumerable<EntityPurposeDefinition> purposes)
		{
			#region input validation

			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			#endregion

			#region input clean-up

			name = name.Trim();

			if(description == null)
				description = string.Empty;
			else
				description = description.Trim();

			#endregion

			if(entityTypes.ContainsKey(name))
				throw new DuplicateNameException();

			IDictionary<string, EntityPurposeSchema> entityTypePurposes =
				new Dictionary<string, EntityPurposeSchema>(purposes.Count());

			// define reusable setting 'self' source
			DefinitionSource selfDefinitionSource = new DefinitionSource(DefinitionSourceType.EntityType, name);

			foreach(EntityPurposeDefinition purposeDefinition in purposes)
			{
				ResolvePurposeSchema(purposeDefinition, entityTypePurposes, selfDefinitionSource);
			}

			entityTypes.Add(
					name,
					new EntityTypeSchema(
						new EntityTypeInfo(name, description),
						this.purposes,
						entityTypePurposes)
				);

			return this;
		}

		/// <summary>
		/// Builds and return settings schema
		/// </summary>
		/// <returns>An instance of <see cref="ISettingsSchema">ISettingSchema</see> which represents the overall settings schema</returns>
		public ISettingsSchema Build()
		{
			return new SettingsSchema(purposes, entityTypes);
		}

		#endregion

		private bool PurposeExists(string name)
		{
			return purposes.ContainsKey(name);
		}

		private bool EntityExists(string name)
		{
			return entityTypes.ContainsKey(name);
		}
	}
}