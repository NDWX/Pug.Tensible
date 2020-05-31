namespace Settings.Schema
{
	public interface ISettingSchema
	{
		DefinitionSource Source { get;  }
		
		bool Inheritable { get; }
		
		SettingDefinition Definition { get;  }
	}
}