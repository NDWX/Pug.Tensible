namespace Settings.Schema
{
	/// <summary>
	/// Source of setting definition. A setting may be defined by an entity type or inherited from a parent entity type.
	/// </summary>
	public class SettingDefinitionSource
	{
		public SettingDefinitionSource(DefinitionSourceType type, string entityType, SettingDefinitionSource source = null)
		{
			this.Type = type;
			this.EntityType = entityType;
			this.Source = source;
		}
		
		/// <summary>
		/// Whether setting was defined by entity itself or inherited from a parent entity type.
		/// </summary>
		public DefinitionSourceType Type { get; }
		
		/// <summary>
		/// Parent entity type of setting was inherited.
		/// </summary>
		public string EntityType { get;  }
		
		/// <summary>
		/// Information about whether setting was defined by parent entity type or inherited from another entity type.
		/// </summary>
		public SettingDefinitionSource Source { get; }
	}
}