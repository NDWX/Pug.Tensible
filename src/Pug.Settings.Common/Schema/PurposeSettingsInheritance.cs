using System.Collections.Generic;
using System.Linq;

namespace Settings.Schema
{
	public class PurposeSettingsInheritance
	{
		public PurposeSettingsInheritanceType InheritanceType { get; }
		public IEnumerable<string> ApplicableSettings { get; }

		public PurposeSettingsInheritance(PurposeSettingsInheritanceType inheritanceType, IEnumerable<string> applicableSettings = null)
		{
			InheritanceType = inheritanceType;
			ApplicableSettings = applicableSettings?.Select(x => x.Trim());
		}
	}
}