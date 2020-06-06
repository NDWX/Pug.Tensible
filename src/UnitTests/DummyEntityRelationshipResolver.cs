using Settings;

namespace UnitTests
{
	public class DummyEntityRelationshipResolver : IEntityRelationshipResolver
	{
		public string GetEntityParent(EntityIdentifier entity, string parentType)
		{
			return null;
		}
	}
}