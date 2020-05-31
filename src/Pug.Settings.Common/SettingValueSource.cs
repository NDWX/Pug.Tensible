namespace Settings
{
	/// <summary>
	/// Describes source of effective setting
	/// </summary>
	public class SettingValueSource
	{
		/// <summary>
		/// Source type
		/// </summary>
		public SettingValueSourceType Type { get; set; }
		
		public string EntityType { get; set; }
	}
}