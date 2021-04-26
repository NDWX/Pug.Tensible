using Pug.Effable;
using Settings;
using Settings.Schema;
using Tensible;

namespace UnitTests
{
	public class TestContext
	{
		public IUnifier Unifier { get; set; } = new Unifier(collection => { });

		public IUnified Unified { get; set; }
	}

	public class OrganizationInfo : NamedEntityInfo<string, string>
	{
		public OrganizationInfo(string identifier, string name) : base(identifier, name)
		{
		}
	}


	public class OrganizationUnitInfo : NamedEntityInfo<string, string>
	{
		public OrganizationUnitInfo(string identifier, string name) : base(identifier, name)
		{
		}

		public string Organization { get; }
	}

	public class UserInfo : NamedEntityInfo<string, string>
	{
		public UserInfo(string identifier, string name) : base(identifier, name)
		{
		}

		public string Organization { get; }

		public string Department { get; }
	}

	public class InterfaceSettings
	{
		public string Theme { get; set; }

		public string Layout { get; set; }
		
		public string LandingPage { get; set; }
	}

	public class OrgUnitInterfaceSettings : InterfaceSettings
	{
		public string Logo { get; set; }
	}

	public class UserInterfaceSettings : InterfaceSettings
	{
		public string Avatar { get; set; }
		
		public string LandingPage { get; set; }
	}

	public class SettingsStore
	{
		public InterfaceSettings GetOrgInterfaceSettings(string identifier)
		{
			return new InterfaceSettings()
			{
				Layout = "Standard",
				Theme = "Light",
				LandingPage = "Default"
			};
		}

		public OrgUnitInterfaceSettings GetOrgUnitInterfaceSettings(string identifier)
		{
			return new OrgUnitInterfaceSettings()
			{
				Layout = "Compact",
				Logo = "logo.png",
				Theme = string.Empty
			};
		}

		public UserInterfaceSettings GetUserInterfaceSettings(string identifier)
		{
			return new UserInterfaceSettings()
			{
				Avatar = "user.png",
				Layout = string.Empty,
				Theme = string.Empty,
				LandingPage = string.Empty
			};
		}
	}
}