using System;
using System.Collections.Generic;
using Settings.Schema;

namespace Pug.Settings
{
	public interface IPurposeDefinition
	{
		string Name { get; }
		
		string ParentEntityType { get; }
		
		PurposeSettingsInheritanceType ParentSettingsInheritance { get; }
		
		PurposeSettingsInheritanceType SettingsInheritance { get; }

		Type RunType { get; }
		
		IDictionary<string, ISettingDefinition> Settings { get; }
	}
}