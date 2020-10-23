using System;
using System.Collections.Generic;
using Settings.Schema;

namespace Pug.Settings.Schema
{
	internal class EntityTypeSettingsSchema : IEntityTypeSettingsSchema
	{
		private readonly ISettingsSchema _schema;
		
		public string Name { get; }
		IDictionary<string, IPurposeDefinition> purposes = new Dictionary<string, IPurposeDefinition>();

		internal EntityTypeSettingsSchema(string name, ISettingsSchema schema)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			
			_schema = schema;

			Name = name;
		}

		public IEntityTypeSettingsSchema With<TPurpose>(Purpose<TPurpose> purpose)
			where TPurpose : class, ISettingsPurpose, new()
		{
			if(purpose == null) throw new ArgumentNullException(nameof(purpose));

			if(purposes.ContainsKey(purpose.Name))
				throw new PurposeException(purpose.Name, $"Purpose name '{purpose.Name}' is already used.");

			// validate inheritance from parent entity type
			if(!string.IsNullOrWhiteSpace(purpose.ParentEntityType) && purpose.ParentSettingsInheritance == PurposeSettingsInheritanceType.Inherit)
			{
				// ensure parent entity type exists
				IEntityTypeSettingsSchema parentSchema = _schema[purpose.ParentEntityType]??
														throw new PurposeException(purpose.Name, $"Parent entity type '{purpose.ParentEntityType}' inherited by purpose does not exist.");

				// ensure parent entity type contains purpose
				IPurposeDefinition parentPurposeDefinition = parentSchema[purpose.Name] ??
															throw new PurposeException(purpose.Name, $"Parent entity type '{purpose.ParentEntityType}' does not contain purpose '{purpose.Name}'.");

				// ensure parent entity type purpose is inheritable
				if( parentPurposeDefinition.SettingsInheritance == PurposeSettingsInheritanceType.DoNotInherit)
					throw new PurposeException(purpose.Name, $"Purpose '{purpose.Name}' of entity type '{purpose.ParentEntityType}' cannot be inherited.");
				
				// ensure inherited setting has matching return type.
				foreach(ISettingDefinition settingDefinition in purpose.Settings.Values)
				{
					if(settingDefinition.ParentInheritance == PurposeSettingsInheritanceType.DoNotInherit)
						continue;

					if(parentPurposeDefinition.Settings.ContainsKey(settingDefinition.Name))
					{
						ISettingDefinition parentSetting = parentPurposeDefinition.Settings[settingDefinition.Name];
						
						if( parentSetting.Inheritability == PurposeSettingsInheritanceType.DoNotInherit)
							throw new SettingException(settingDefinition.Name, $"Setting '{settingDefinition.Name}' or parent entity type '{purpose.ParentEntityType}' cannot be inherited.");
						
						if( !parentSetting.ValueType.InheritsOrEqualsTo(settingDefinition.ValueType) )
							throw new SettingException(settingDefinition.Name, $"Setting '{settingDefinition.Name}' or parent entity type '{purpose.ParentEntityType}' returns type '{parentSetting.ValueType.FullName}' which does not inherit or equal to '{settingDefinition.ValueType.FullName}'.");
					}
				}
			}

			purposes.Add(purpose.Name, purpose);

			return this;
		}

		public IPurposeDefinition this[string purpose]
		{
			get
			{
				if(purposes.ContainsKey(purpose))
					return purposes[purpose];

				return null;
			}
		}
	}
}