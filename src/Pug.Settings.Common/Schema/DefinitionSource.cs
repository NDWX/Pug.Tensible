namespace Settings.Schema
{
	public class DefinitionSource
	{
		public DefinitionSource(DefinitionSourceType type, string entityType, DefinitionSource source = null)
		{
			this.Type = type;
			this.EntityType = entityType;
			this.Source = source;
		}
		
		public DefinitionSourceType Type { get; }
		
		public string EntityType { get;  }
		
		public DefinitionSource Source { get; }
	}
}