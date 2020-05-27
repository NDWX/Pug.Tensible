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

			purposes.Add(name, new PurposeInfo(name, description, Inheritability.None, false));

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
												IDictionary<string, IEnumerable<SettingDefinition>> purposeSettings)
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

			IDictionary<string, EntityTypePurpose> entityTypePurposes = new Dictionary<string, EntityTypePurpose>();

			foreach(KeyValuePair<string, IEnumerable<SettingDefinition>> purpose in purposeSettings)
			{
				string purposeKey = purpose.Key.Trim();
				
				if( purposeKey != string.Empty && entityTypePurposes.ContainsKey(purposeKey))
					throw new DuplicateNameException($"Duplicate purpose name '{purposeKey}' specified for entity type");
				
				if(!this.purposes.ContainsKey(purposeKey))
					throw new UnknownPurpose(purposeKey);

				IEnumerable<SettingDefinition> settingDefinitions = purpose.Value;

				// determine duplicated setting names in 'purpose'
				IEnumerable<IGrouping<string, SettingDefinition>> duplicatedNames =
					from settingDefinition in settingDefinitions
					group settingDefinition by settingDefinition.Name
					into nameDefinitions
					where nameDefinitions.Count() > 1
					select nameDefinitions;

				if(duplicatedNames.Any())
					throw new DuplicateNameException(duplicatedNames.First().Key);

				entityTypePurposes.Add(purposeKey,
										new EntityTypePurpose(purposeKey, purpose.Value.ToDictionary(x => x.Name)));
			}

			entityTypes.Add(
				name, new EntityTypeSchema(new EntityTypeInfo(name, description), this.purposes, entityTypePurposes));

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