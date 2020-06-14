using System;
using System.Collections.Generic;

namespace Settings
{
	/// <summary>
	/// Interface for effective setting/s resolver
	/// </summary>
	public interface IResolver
	{
		/// <summary>
		/// Resolve all effective settings within a purpose for the specified entity.
		/// </summary>
		/// <param name="entity">Entity to for which settings are to be resolved</param>
		/// <param name="purpose">The purpose of the settings which are to be resolved</param>
		/// <returns>All effective settings for the specified entity and purpose</returns>
		/// <exception cref="UnknownEntityType">When specified entity type is not known within settings schema</exception>
		IDictionary<string, Setting> ResolveSettings(EntityIdentifier entity, string purpose);

		/// <summary>
		/// Resolve effective setting within the purpose for the specified entity
		/// </summary>
		/// <param name="entity">Entity to for which settings are to be resolved</param>
		/// <param name="purpose">The purpose of the settings which are to be resolved</param>
		/// <param name="name">Name of the setting for the specified entity and purpose</param>
		/// <returns>Effective setting for the specified entity, purpose, and name</returns>
		/// <exception cref="ArgumentException">One of the specified arguments fail validation</exception>
		/// <exception cref="UnknownEntityType">Specified entity type is not known within settings schema</exception>
		/// <exception cref="UnknownSetting">Specified setting name is not known within settings schema</exception>
		Setting ResolveSetting(EntityIdentifier entity, string purpose, string name);
	}
}