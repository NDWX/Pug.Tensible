using System.Collections.Generic;

namespace Settings.Schema
{
	public interface IEntityPurposeSchema
	{
		DefinitionSource Source { get;  }
		
		EntityPurposeDefinition Definition { get;  }
		
		IDictionary<string, ISettingSchema> Settings { get;  }
	}
}