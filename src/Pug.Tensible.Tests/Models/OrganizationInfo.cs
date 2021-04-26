using Pug.Effable;

namespace Pug.Tensible.Tests
{
	public class OrganizationInfo : NamedEntityInfo<string, string>
	{
		public OrganizationInfo(string identifier, string name) : base(identifier, name)
		{
		}
	}
}