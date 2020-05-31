namespace Settings.Schema
{
	public class DefinitionSource
	{
		public DefinitionSource(DefinitionSourceType type, string entityType)
		{
			this.Type = type;
			this.EntityType = entityType;
		}
		
		public DefinitionSourceType Type { get; }
		
		public string EntityType { get;  }
	}
}