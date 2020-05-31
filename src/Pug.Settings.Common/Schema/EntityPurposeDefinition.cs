using System;
using System.Collections.Generic;

namespace Settings.Schema
{
	public class EntityPurposeDefinition
	{
		public string Name { get; }
		public string ParentEntityType { get; }
		public PurposeSettingsInheritanceType PurposeSettingsInheritanceType { get; }
		public PurposeSettingsInheritance Inheritance { get; }
		public IEnumerable<SettingDefinition> Settings { get; }
		public PurposeSettingsInheritance Inheritability { get; }

		public EntityPurposeDefinition(string name, string parentEntityType, PurposeSettingsInheritance inheritance, 
										IEnumerable<SettingDefinition> settings, PurposeSettingsInheritance inheritability)
		{
			Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
			ParentEntityType = parentEntityType?.Trim();
			Inheritance = inheritance;
			Settings = settings;
			Inheritability = inheritability;
		}
	}
}