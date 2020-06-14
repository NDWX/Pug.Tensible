namespace Settings.Schema
{
	/// <summary>
	/// Represents schema of a setting
	/// </summary>
	public interface ISettingSchema
	{
		/// <summary>
		/// Source of setting definition. <see cref="SettingDefinitionSource"/>
		/// </summary>
		SettingDefinitionSource Source { get;  }
		
		/// <summary>
		/// Whether setting may be inherited. Value of this property is determined based on <see cref="PurposeSettingsInheritance"/> specified during entity type registration.
		/// </summary>
		bool Inheritable { get; }
		
		/// <summary>
		/// Definition of setting on which this schema was based.
		/// </summary>
		SettingDefinition Definition { get;  }
	}
}