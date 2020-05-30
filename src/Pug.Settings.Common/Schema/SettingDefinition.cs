namespace Settings.Schema
{
	/// <summary>
	/// Definition of setting
	/// </summary>
	public class SettingDefinition : ElementInfo
	{
		public SettingDefinition(string name, string description, bool inheritable, bool hasDefaultValue, string defaultValue = null)
			: base(name, description)
		{
			Inheritable = inheritable;
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
		}

		public bool Inheritable { get; }

		/// <summary>
		/// Whether default value is specified
		/// </summary>
		public bool HasDefaultValue { get; protected set; }

		/// <summary>
		/// Default value of setting
		/// </summary>
		public string DefaultValue { get; protected set; }
	}
}