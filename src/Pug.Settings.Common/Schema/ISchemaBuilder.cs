using System;
using System.Collections.Generic;

namespace Settings.Schema
{
	public interface ISchemaBuilder
	{
		/// <summary>
		/// Implementation must register a setting purpose and return itself.
		/// Implementer should also make sure call to this function throws <see cref="InvalidOperationException"/> if <see cref="Build"/> was already called.
		/// </summary>
		/// <param name="name">Name of the purpose</param>
		/// <param name="description">Description of purpose</param>
		/// <returns>The same instance of a schema builder</returns>
		/// <exception cref="ArgumentException">Name or description is null or empty</exception>
		/// <exception cref="InvalidOperationException">Schema was already built</exception>
		/// <exception cref="DuplicateNameException">Name of purpose has already been used by another purpose</exception>
		ISchemaBuilder RegisterPurpose(string name, string description);

		/// <summary>
		/// Register an entity type with all its purposes and settings, and return itself.
		/// Implementer should make sure call to this function throws <see cref="InvalidOperationException"/> if <see cref="Build"/> was already called.
		/// </summary>
		/// <param name="name">Name of entity type</param>
		/// <param name="description">Description of entity type</param>
		/// <param name="purposeSettings">Entity purposes and settings</param>
		/// <returns>The same instance of a schema builder</returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException">Schema was already built</exception>
		/// <exception cref="DuplicateNameException">Entity, or purpose within entity or setting within a purpose is duplicated</exception>
		/// <exception cref="UnknownPurpose">Purpose has not been pre-registered using <see cref="RegisterPurpose">RegisterPurpose</see></exception>
		ISchemaBuilder RegisterEntityType(string name, string description,
										IEnumerable<EntityPurposeDefinition> purposes);

		/// <summary>
		///Implementation must return final overall settings schema
		/// </summary>
		/// <returns>An instance of <see cref="ISettingsSchema">ISettingSchema</see> which represents the overall settings schema</returns>
		ISettingsSchema Build();
	}
}