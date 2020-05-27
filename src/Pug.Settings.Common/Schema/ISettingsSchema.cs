using System.Collections.Generic;

namespace Settings.Schema
{
	/// <summary>
	/// Represents setting schema 
	/// </summary>
	public interface ISettingsSchema
	{
		/// <summary>
		/// Get infos of all registered purposes
		/// </summary>
		/// <returns>Entity type infos or empty enumerable</returns>
		IEnumerable<EntityTypeInfo> GetEntityTypes();

		/// <summary>
		/// Get infos of all registered purposes
		/// </summary>
		/// <returns>Purpose infos or empty enumerable</returns>
		IEnumerable<PurposeInfo> GetPurposes();

		/// <summary>
		/// Get entity type by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns>An instance of <see ref="IEntityType">IEntityType</see> or null</returns>
		IEntityType GetEntityType(string name);
	}
}