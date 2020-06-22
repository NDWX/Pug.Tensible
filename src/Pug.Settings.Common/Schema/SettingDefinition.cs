namespace Settings.Schema
{
	/// <summary>
	/// Definition of setting
	/// </summary>
	public class SettingDefinition : ElementInfo
	{
		public SettingDefinition(string name, string description, bool hasDefaultValue = false, string defaultValue = null)
			: base(name, description)
		{
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
		}

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