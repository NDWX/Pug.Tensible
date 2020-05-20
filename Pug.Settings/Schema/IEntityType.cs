using System.Collections.Generic;

namespace Settings.Schema
{
	public interface IEntityType
	{
		EntityTypeInfo GetInfo();

		IEnumerable<PurposeInfo> GetPurposes();

		IEnumerable<SettingDefinition> GetSettings(string purpose = null, string name = null);
	}
}