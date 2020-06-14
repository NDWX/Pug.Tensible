namespace Settings
{
	/// <summary>
	/// Describes source of effective setting
	/// </summary>
	public class SettingValueSource
	{
		public SettingValueSource(SettingValueSourceType type, EntityIdentifier entity, SettingValueSource source = null)
		{
			this.Type = type;
			this.Entity = entity;
			this.Source = source;
		}
		
		/// <summary>
		/// Source type
		/// </summary>
		public SettingValueSourceType Type { get;  }
		
		/// <summary>
		/// The entity from which effective setting value is obtained
		/// </summary>
		public EntityIdentifier Entity { get;  }

		/// <summary>
		/// Information about the source from which Parent entity obtained effective setting value
		/// </summary>
		public SettingValueSource Source { get;  }
	}
}