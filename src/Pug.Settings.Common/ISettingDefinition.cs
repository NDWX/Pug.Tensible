using System;
using System.Reflection;
using Settings.Schema;

namespace Pug.Settings
{
	public interface ISettingDefinition
	{
		string Name { get; }
		
		PropertyInfo Declaration { get; }
		
		Type ValueType { get; }
		
		bool HasDefaultValue { get; }
		
		object DefaultValue { get; }
		
		PurposeSettingsInheritanceType ParentInheritance { get;  }
		
		PurposeSettingsInheritanceType Inheritability { get;  }
	}
}