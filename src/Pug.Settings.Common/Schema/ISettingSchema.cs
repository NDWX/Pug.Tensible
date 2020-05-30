namespace Settings.Schema
{
	public interface ISettingSchema
	{
		DefinitionSource Source { get;  }
		
		SettingDefinition Definition { get;  }
	}
}