using System;

namespace Settings
{
	[Flags]
	public enum SettingValueSourceType
	{
		/// <summary>
		/// Value of effective setting was obtained from default value specified in <see cref="Schema.SettingDefinition">SettingDefinition</see>
		/// </summary>
		Default = 1,
		
		/// <summary>
		/// Value of effective setting was obtained from parent
		/// </summary>
		Parent = 2,
		
		/// <summary>
		/// Value of effective setting was specified by user
		/// </summary>
		/// 
		User = 4
	}
}