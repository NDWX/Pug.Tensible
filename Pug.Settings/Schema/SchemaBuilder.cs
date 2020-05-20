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
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			if(description == null) throw new ArgumentNullException(nameof(description));

			if(purposes.ContainsKey(name))
				throw new DuplicateNameException();

			purposes.Add(name, new PurposeInfo(name, description, Inheritability.None, false));

			return this;
		}

		public ISchemaBuilder RegisterEntityType(string name, string description,
												IDictionary<string, IEnumerable<SettingDefinition>> purposes)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			if(description == null) throw new ArgumentNullException(nameof(description));

			if(entityTypes.ContainsKey(name))
				throw new DuplicateNameException();

			foreach(KeyValuePair<string, IEnumerable<SettingDefinition>> purpose in purposes)
			{
				if(!this.purposes.ContainsKey(purpose.Key))
					throw new UnknownPurpose(purpose.Key);

				IEnumerable<SettingDefinition> purposeSettings = purpose.Value;

				IEnumerable<IGrouping<string, SettingDefinition>> duplicatedNames =
					from settingDefinition in purposeSettings
					group settingDefinition by settingDefinition.Name
					into nameDefinitions
					where nameDefinitions.Count() > 1
					select nameDefinitions;

				if(duplicatedNames.Count() > 0)
					throw new DuplicateNameException(duplicatedNames.First().Key);
			}

			entityTypes.Add(name, new EntityTypeSchema(new EntityTypeInfo(name, description), this.purposes, purposes));

			return this;
		}

		#endregion

		private bool PurposeExsists(string name)
		{
			return purposes.ContainsKey(name);
		}

		private bool EntityExists(string name)
		{
			return entityTypes.ContainsKey(name);
		}

		public ISettingsSchema Build()
		{
			return new SettingsSchema(purposes, entityTypes);
		}
	}
}