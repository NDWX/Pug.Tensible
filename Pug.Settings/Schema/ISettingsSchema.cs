using System.Collections.Generic;

namespace Settings.Schema
{
	public interface ISettingsSchema
	{
		IEnumerable<EntityTypeInfo> GetEntityTypes(string parent);

		IEnumerable<PurposeInfo> GetPurposes();

		IEntityType GetEntityType(string name);
	}
}