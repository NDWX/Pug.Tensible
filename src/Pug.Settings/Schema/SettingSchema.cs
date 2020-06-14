namespace Settings.Schema
{
	class SettingSchema : ISettingSchema
	{
		public SettingDefinitionSource Source { get; set; }
		
		public bool Inheritable { get; set; }
		
		public SettingDefinition Definition { get; set; }
	}
}