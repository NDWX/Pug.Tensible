namespace Settings
{
	public interface IEntityRelationshipResolver
	{
		string GetEntityParent(EntityIdentifier entity, string parentType);
	}
}