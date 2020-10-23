using System;
using Settings.Schema;

namespace Pug.Settings.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SettingAttribute : Attribute
	{
		public SettingAttribute(string description, string parentSettingName = null, PurposeSettingsInheritanceType parentInheritanceType = PurposeSettingsInheritanceType.Unspecified, bool hasDefaultValue = false, object defaultValue = null, PurposeSettingsInheritanceType inheritability = PurposeSettingsInheritanceType.Unspecified)
		{
			this.Description = description ?? throw new ArgumentNullException(nameof(description));
			ParentSettingName = parentSettingName;
			ParentInheritanceType = parentInheritanceType;
			this.HasDefaultValue = hasDefaultValue;
			this.DefaultValue = defaultValue;
			Inheritability = inheritability;
		}
		
		public string Description { get; }
		
		public string ParentSettingName { get; }

		public PurposeSettingsInheritanceType ParentInheritanceType { get; }

		public bool HasDefaultValue { get; }
		
		public object DefaultValue { get; }
		
		public PurposeSettingsInheritanceType Inheritability { get; }
	}
}