namespace Settings.Schema
{
	class SettingSchema : ISettingSchema
	{
		public DefinitionSource Source { get; set; }
		
		public SettingDefinition Definition { get; set; }
	}
}