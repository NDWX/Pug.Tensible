using System.Collections.Generic;

namespace Settings.Schema
{
	class EntityPurposeSchema : IEntityPurposeSchema
	{
		public DefinitionSource Source { get; set; }
		
		public EntityPurposeDefinition Definition { get; set; }
		
		public IDictionary<string, ISettingSchema> Settings { get; set; }
	}
}