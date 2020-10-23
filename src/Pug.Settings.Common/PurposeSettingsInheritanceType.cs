namespace Settings.Schema
{
	public enum PurposeSettingsInheritanceType
	{
		Unspecified,
		
		/// <summary>
		/// Indicates setting/s are not inheritable or should be inherited
		/// </summary>
		DoNotInherit,
		
		/// <summary>
		/// Indicates settings ares inheritable or hould be inherited
		/// </summary>
		Inherit,
	}
}