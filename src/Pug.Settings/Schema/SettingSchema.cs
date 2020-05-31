namespace Settings.Schema
{
	class SettingSchema : ISettingSchema
	{
		public DefinitionSource Source { get; set; }
		
		public bool Inheritable { get; set; }
		
		public SettingDefinition Definition { get; set; }
	}
}