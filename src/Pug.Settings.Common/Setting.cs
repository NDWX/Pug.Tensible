namespace Settings
{
	public class Setting
	{
		public string Purpose { get; set; }
		public string Name { get; set; }
		public SettingValueSource ValueSource { get; set; }
		public string Value { get; set; }

		// public DateTime LastUpdateTimestamp { get; set; }
		//
		// public string LastUpdateUser { get; set; }
	}
}