using System.Collections.Generic;

namespace Settings
{
	/// <summary>
	/// Implementation of this interface provides read-only access to application settings
	/// </summary>
	public interface ISettingStore
	{
		/// <summary>
		/// Get entity setting for specified purpose
		/// </summary>
		/// <param name="entity">The entity for which the setting is to be retrieved</param>
		/// <param name="purpose">Purpose of the setting</param>
		/// <param name="name">Name of the setting</param>
		/// <returns>Instance of setting as stored or null if setting is not found within store</returns>
		Setting GetSetting(EntityIdentifier entity, string purpose, string name);
		
		/// <summary>
		/// Get entity settings for specified purpose
		/// </summary>
		/// <param name="entity">The entity for which the setting is to be retrieved</param>
		/// <param name="purpose">Purpose of the setting</param>
		/// <returns>All stored settings for entity and purpose or empty list if none is stored</returns>
		IEnumerable<Setting> GetSettings(EntityIdentifier entity, string purpose);
	}
}