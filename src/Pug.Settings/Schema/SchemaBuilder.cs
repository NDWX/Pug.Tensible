using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Settings.Schema
{
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

		public ISchemaBuilder RegisterEntityType(string name, string description,
												IDictionary<string, IEnumerable<SettingDefinition>> purposes)
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

			foreach(KeyValuePair<string, IEnumerable<SettingDefinition>> purpose in purposes)
			{
				string purposeKey = purpose.Key.Trim();
				
				if( purposeKey != string.Empty && entityTypePurposes.ContainsKey(purposeKey))
					throw new DuplicateNameException($"Duplicate purpose name '{purposeKey}' specified for entity type");
				
				if(!this.purposes.ContainsKey(purposeKey))
					throw new UnknownPurpose(purposeKey);

				IEnumerable<SettingDefinition> purposeSettings = purpose.Value;

				// determine duplicated setting names in 'purpose'
				IEnumerable<IGrouping<string, SettingDefinition>> duplicatedNames =
					from settingDefinition in purposeSettings
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