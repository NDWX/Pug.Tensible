namespace Settings
{
	/// <summary>
	/// Describes source of effective setting
	/// </summary>
	public class SettingValueSource
	{
		public SettingValueSource(SettingValueSourceType type, string entityType, SettingValueSource source = null)
		{
			this.Type = type;
			this.EntityType = entityType;
			this.Source = source;
		}
		
		/// <summary>
		/// Source type
		/// </summary>
		public SettingValueSourceType Type { get;  }
		
		public string EntityType { get;  }

		public SettingValueSource Source { get;  }
	}
}