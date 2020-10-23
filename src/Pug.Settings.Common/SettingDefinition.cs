using System;
using System.Linq;
using System.Reflection;
using Settings.Schema;

namespace Pug.Settings
{
	internal class SettingDefinition : ISettingDefinition
	{
		public SettingDefinition(PropertyInfo declaration, bool hasDefaultValue = false, object defaultValue = null,
								PurposeSettingsInheritanceType parentInheritance =
									PurposeSettingsInheritanceType.Unspecified,
								PurposeSettingsInheritanceType inheritability =
									PurposeSettingsInheritanceType.Unspecified)
		{
			Name = declaration.Name;
			Declaration = declaration;
			this.ValueType = declaration.PropertyType.GetGenericArguments().First();
			this.HasDefaultValue = hasDefaultValue;
			this.DefaultValue = defaultValue;
			this.ParentInheritance = parentInheritance;
			this.Inheritability = inheritability;
		}

		public string Name { get; }
		
		public PropertyInfo Declaration { get; }
		
		public Type ValueType { get; }
		public bool HasDefaultValue { get; }
		public object DefaultValue { get; }
		public PurposeSettingsInheritanceType ParentInheritance { get;  }
		public PurposeSettingsInheritanceType Inheritability { get;  }
	}
}