namespace Settings
{
	/// <summary>
	/// Where settings are inherited by a child entity/type from parent entity/type, an impelementation of this interface will be used to identify parent entity of a child entity.
	/// </summary>
	public interface IEntityRelationshipResolver
	{
		/// <summary>
		/// Identifies parent entity of a child entity
		/// </summary>
		/// <param name="entity">Child entity</param>
		/// <param name="parentType">Parent entity type</param>
		/// <returns></returns>
		string GetEntityParent(EntityIdentifier entity, string parentType, string purpose = null);
	}
}