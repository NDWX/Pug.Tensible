using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	/// <summary>
	/// This class encapsulates developer intention regarding entity type settings inheritability or inheritance preference.
	/// Parent entity type uses instance of this this class is used to indicate which setting/s may or may not be inherited by child entity type.
	/// Child entity type uses instance of this class to indicate whether eo inherit or exclude inheritable settings from parent entity type.
	/// </summary>
	public class PurposeSettingsInheritance
	{
		/// <summary>
		/// See <see cref="PurposeSettingsInheritanceType"/>
		/// </summary>
		public PurposeSettingsInheritanceType InheritanceType { get; }
		
		/// <summary>
		/// List of applicable settings based on InheritanceType.
		/// </summary>
		public IEnumerable<string> ApplicableSettings { get; }

		public PurposeSettingsInheritance(PurposeSettingsInheritanceType inheritanceType, IEnumerable<string> applicableSettings = null)
		{
			InheritanceType = inheritanceType;
			ApplicableSettings = applicableSettings?.Select(x => x.Trim());
		}
	}
}