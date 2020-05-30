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

			foreach(EntityPurposeDefinition purposeDefinition in purposes)
			{
				string purposeKey = purposeDefinition.Name;

				if(purposeKey != string.Empty && entityTypePurposes.ContainsKey(purposeKey))
					throw new DuplicateNameException(
						$"Duplicate purpose name '{purposeKey}' specified for entity type");

				// ensure purpose info exists
				if(!this.purposes.ContainsKey(purposeKey))
					throw new UnknownPurpose(purposeKey);

				EntityTypeSchema parentSchema = null;
				DefinitionSource parentDefinitionSource = null;

				// check parent entity type and purpose inheritability
				if(!string.IsNullOrWhiteSpace(purposeDefinition.ParentEntityType))
				{
					// ensure parent entity type is known
					if(!entityTypes.ContainsKey(purposeDefinition.ParentEntityType))
						throw new UnknownEntityType(purposeDefinition.ParentEntityType);

					// get parent entity type schema
					parentSchema = entityTypes[purposeDefinition.ParentEntityType];

					EntityPurposeSchema parentPurpose = parentSchema.GetPurpose(purposeDefinition.Name);
					
					// check purpose exists in parent entity type
					if( parentPurpose == null )
						throw new UnknownPurpose(purposeDefinition.Name, $"Parent entity type '{parentSchema.Info.Name}' does not have purpose '{purposeDefinition.Name}'");

					// check parent entity type purpose is inheritable
					if(parentPurpose.Definition.Inheritability == Inheritability.NotInheritable)
						throw new NotInheritable("Purpose '{purposeDefinition.Name}' of entity type '{parentSchema.Info.Name}' is not inheritable");

					// define reusable setting 'parent' source
					parentDefinitionSource =
						new DefinitionSource(DefinitionSourceType.ParentEntityType, purposeDefinition.ParentEntityType);
				}

				// determine duplicated setting names in 'purpose'
				IEnumerable<IGrouping<string, SettingDefinition>> duplicatedNames =
					from settingDefinition in purposeDefinition.Settings
					group settingDefinition by settingDefinition.Name
					into nameDefinitions
					where nameDefinitions.Count() > 1
					select nameDefinitions;

				if(duplicatedNames.Any())
					throw new DuplicateNameException(duplicatedNames.First().Key);

				#region resolve 'effective' SettingSchemas

				IDictionary<string, SettingSchema> settingDefinitions =
					new Dictionary<string, SettingSchema>(purposeDefinition.Settings?.Count() ?? 0);

				// define reusable setting 'self' source
				DefinitionSource selfDefinitionSource = new DefinitionSource(DefinitionSourceType.EntityType, name);

				// include self defined settings
				foreach(SettingDefinition settingDefinition in purposeDefinition.Settings)
				{
					settingDefinitions.Add(
							settingDefinition.Name,
							new SettingSchema()
							{
								Source = selfDefinitionSource,
								Definition = settingDefinition
							}
						);
				}

				// resolve inherited settings
				if(parentSchema != null)
				{
					foreach(ISettingSchema settingSchema in parentSchema.GetSettings(purposeDefinition.Name))
					{
						// ensure setting is inheritable
						if(!settingSchema.Definition.Inheritable)
							continue;

						// skip if setting is already defined by 'self' 
						if(settingDefinitions.ContainsKey(settingSchema.Definition.Name))
							continue;

						settingDefinitions.Add(
								settingSchema.Definition.Name,
								new SettingSchema()
								{
									// determine whether source is direct parent or inherited by parent
									Source = settingSchema.Source.Type == DefinitionSourceType.EntityType? parentDefinitionSource : settingSchema.Source,
									Definition = settingSchema.Definition
								}
							);
					}
				}

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