using System;
using Settings.Schema;

namespace Pug.Settings.Annotations
{
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct , AllowMultiple = false, Inherited = true)]
	public class PurposeAttribute : Attribute
	{
		public string Description { get; }

		public PurposeSettingsInheritanceType ParentSettingsInheritance { get; }
		
		public PurposeSettingsInheritanceType Inheritability { get; }

		public PurposeAttribute(string description, PurposeSettingsInheritanceType parentSettingsInheritance = PurposeSettingsInheritanceType.Inherit, PurposeSettingsInheritanceType inheritability = PurposeSettingsInheritanceType.Inherit )
		{
			Description = description ?? throw new ArgumentNullException(nameof(description));
			ParentSettingsInheritance = parentSettingsInheritance;
			Inheritability = inheritability;
		}
	}
}