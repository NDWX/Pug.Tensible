using Pug.Effable;

namespace Pug.Tensible.Tests
{
	public class OrganizationUnitInfo : NamedEntityInfo<string, string>
	{
		public OrganizationUnitInfo(string identifier, string name) : base(identifier, name)
		{
		}

		public string Organization { get; }
	}
}