using System.Collections.Generic;

namespace Settings.Schema
{
	public interface ISchemaBuilder
	{
		ISchemaBuilder RegisterPurpose(string name, string description);

		ISchemaBuilder RegisterEntityType(string name, string description,
										IDictionary<string, IEnumerable<SettingDefinition>> purposes);

		ISettingsSchema Build();
	}
}