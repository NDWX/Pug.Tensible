using System.Collections.Generic;

namespace Settings.Schema
{
	/// <summary>
	/// Represents an entity type schema
	/// </summary>
	public interface IEntityTypeSchema
	{
		/// <summary>
		/// Gets info of the entity type
		/// </summary>
		/// <returns>Info of the entity type</returns>
		EntityTypeInfo GetEntityTypeInfo();

		/// <summary>
		/// Get infos for all purposes within the entity type
		/// </summary>
		/// <returns>Purpose infos</returns>
		IEnumerable<PurposeInfo> GetPurposes();

		IEntityPurposeSchema GetPurpose(string name);

		/// <summary>
		/// Get all setting definitions within purpose or setting definition as specified by purpose and name
		/// </summary>
		/// <param name="purpose">Purpose of setting</param>
		/// <param name="name">Name of setting</param>
		/// <returns>One or more setting definitions</returns>
		IEnumerable<ISettingSchema> GetSettings(string purpose = null, string name = null);
	}
}