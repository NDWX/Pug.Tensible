using Settings;
using Settings.Schema;
using Tensible;

namespace Pug.Tensible.Tests
{
	public class TestContext
	{
		public IUnifier Unifier { get; set; } = new Unifier(collection => { });

		public IUnified Unified { get; set; }
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