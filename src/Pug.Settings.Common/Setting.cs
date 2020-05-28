namespace Settings
{
	/// <summary>
	/// Represent an effective setting
	/// </summary>
	public class Setting
	{
		/// <summary>
		/// Purpose of setting
		/// </summary>
		public string Purpose { get; set; }
		
		/// <summary>
		/// Name of setting
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Source of setting <see cref="Value">value</see>
		/// </summary>
		public SettingValueSource ValueSource { get; set; }
		
		/// <summary>
		/// Resolved value of setting
		/// </summary>
		public string Value { get; set; }

		// public DateTime LastUpdateTimestamp { get; set; }
		//
		// public string LastUpdateUser { get; set; }
	}
}