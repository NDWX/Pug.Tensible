using Settings.Schema;

namespace Pug.Settings.Schema
{
	public static class IEntityTypeSettingsSchemaExtensions
	{
		public static IEntityTypeSettingsSchema With<TPurpose>(this IEntityTypeSettingsSchema entityTypeSettingsSchema,
																string name = null, string parentEntityType = null,
																PurposeSettingsInheritanceType parentSettingsInheritance
																	= PurposeSettingsInheritanceType.Unspecified,
																PurposeSettingsInheritanceType settingsInheritance =
																	PurposeSettingsInheritanceType.Unspecified)
			where TPurpose : class, ISettingsPurpose, new()
		{
			if(name == null)
			{
				name = typeof(TPurpose).Name;

				if(name.ToUpper().EndsWith("Settings"))
					name = name.Substring(0, name.Length - 8);
			}
			
			return entityTypeSettingsSchema.With(
					new Purpose<TPurpose>(name, parentEntityType, parentSettingsInheritance,
													settingsInheritance)
				);
		}
		
		/*
		public static IEntityTypeSettingsSchema With<TPurpose>(this IEntityTypeSettingsSchema entityTypeSettingsSchema,
																string parentEntityType = null,
																PurposeSettingsInheritanceType parentSettingsInheritance
																	=
																	PurposeSettingsInheritanceType.Unspecified,
																PurposeSettingsInheritanceType settingsInheritance =
																	PurposeSettingsInheritanceType.Unspecified)
			where TPurpose : class, ISettingsPurpose, new()
		{
			string name = typeof(TPurpose).Name;

			if(name.ToUpper().EndsWith("Settings"))
				name = name.Substring(0, name.Length - 8);
			
			return entityTypeSettingsSchema.With(
					new Purpose<TPurpose>(name, parentEntityType, parentSettingsInheritance,
										settingsInheritance)
				);
		}*/
	}
}