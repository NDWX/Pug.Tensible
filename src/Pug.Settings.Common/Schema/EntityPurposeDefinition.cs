using System;
using System.Collections.Generic;

namespace Settings.Schema
{
	public class EntityPurposeDefinition
	{
		public string Name { get; }
		public string ParentEntityType { get; }
		public Inheritability Inheritability { get; }
		public bool AllowInheritabilityOverride { get; }
		public IEnumerable<SettingDefinition> Settings { get; }

		public EntityPurposeDefinition(string name, string parentEntityType, Inheritability inheritability, 
										bool allowInheritabilityOverride,
										IEnumerable<SettingDefinition> settings)
		{
			Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
			ParentEntityType = parentEntityType?.Trim();
			Inheritability = inheritability;
			AllowInheritabilityOverride = allowInheritabilityOverride;
			Settings = settings;
		}
	}
}