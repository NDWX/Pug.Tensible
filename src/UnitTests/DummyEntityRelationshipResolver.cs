using Settings;

namespace UnitTests
{
	public class DummyEntityRelationshipResolver : IEntityRelationshipResolver
	{
		public string GetEntityParent(EntityIdentifier entity, string parentType)
		{
			if(entity.Type == "SecondType" && entity.Identifier == "SecondEntity" && parentType == "FirstType")
				return "FirstEntity";
			
			if(entity.Type == "ThirdType" && entity.Identifier == "ThirdEntity" && parentType == "SecondType")
				return "SecondEntity";

			return null;
		}
	}
}