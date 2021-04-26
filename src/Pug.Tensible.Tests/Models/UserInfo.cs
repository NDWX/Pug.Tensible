using Pug.Effable;

namespace Pug.Tensible.Tests
{
	public class UserInfo : NamedEntityInfo<string, string>
	{
		public UserInfo(string identifier, string name) : base(identifier, name)
		{
		}

		public string Organization { get; }

		public string Department { get; }
	}
}