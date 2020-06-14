using System.Collections.Generic;

namespace Settings.Schema
{
	/// <summary>
	/// Represent schema of a purpose specified within an entity type
	/// </summary>
	public interface IEntityPurposeSchema
	{
		/// <summary>
		/// Definition of entity purpose on which this schema was based.
		/// </summary>
		EntityPurposeDefinition Definition { get;  }
		
		/// <summary>
		/// Table of settings within this purpose. This table contains both settings that are defined at entity type level and those that are inherited from a parent entity type. 
		/// </summary>
		IDictionary<string, ISettingSchema> Settings { get;  }
	}
}