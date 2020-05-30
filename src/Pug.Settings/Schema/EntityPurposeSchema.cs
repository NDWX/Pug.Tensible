using System.Collections.Generic;

namespace Settings.Schema
{
	class EntityPurposeSchema
	{
		public DefinitionSource Source { get; set; }
		
		public EntityPurposeDefinition Definition { get; set; }
		
		public IDictionary<string, SettingSchema> Settings { get; set; }
	}
}