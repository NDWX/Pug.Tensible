namespace Settings.Schema
{
	public class SettingDefinition : ElementInfo
	{
		public SettingDefinition(string name, string description, bool hasDefaultValue, string defaultValue = null)
			: base(name, description)
		{
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
		}

		public bool HasDefaultValue { get; protected set; }

		public string DefaultValue { get; protected set; }
	}
}