using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Settings.Schema
{
	public class EntityPurposeDefinition
	{
		public string Name { get; }
		public string ParentEntityType { get; }
		public PurposeSettingsInheritance Inheritance { get; }
		public IEnumerable<SettingDefinition> Settings { get; }
		public PurposeSettingsInheritance Inheritability { get; }

		public EntityPurposeDefinition(string name, string parentEntityType, 
										PurposeSettingsInheritance inheritance, 
										IEnumerable<SettingDefinition> settings, 
										PurposeSettingsInheritance inheritability = null)
		{
			Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
			ParentEntityType = parentEntityType?.Trim();

			if(string.IsNullOrEmpty(ParentEntityType))
				Inheritance = null;
			else
			{
				if( inheritance != null )
					Inheritance = inheritance;
				else
					Inheritance = new PurposeSettingsInheritance(PurposeSettingsInheritanceType.Inherit);
			}

			Settings = settings ?? new SettingDefinition[0];
			
			if( inheritability != null )
				Inheritability = inheritability;
			else
				Inheritability = new PurposeSettingsInheritance(PurposeSettingsInheritanceType.DoNotInherit);
		}

		public EntityPurposeDefinition(string name,
										IEnumerable<SettingDefinition> settings,
										PurposeSettingsInheritance inheritability = null) 
			: this(name, null, null, settings, inheritability)
		{
		}
	}
}