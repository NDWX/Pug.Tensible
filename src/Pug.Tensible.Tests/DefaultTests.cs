using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Settings;
using Settings.Schema;
using Xunit;
using Xunit.Extensions.Ordering;
// ReSharper disable HeapView.ClosureAllocation

namespace Pug.Tensible.Tests
{
	[TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
	[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
	public class DefaultTests : IClassFixture<TestContext>
	{
		#region Setup/Teardown

		public DefaultTests(TestContext context)
		{
			this.context = context;
		}

		#endregion

		private readonly TestContext context;

		[Fact]
		public void FirstEntityRegistrationShouldSucceed()
		{
			IUnifier unifier = context.Unifier;
			
			unifier.AddEntity<OrganizationInfo>()
					.With(
							new Settings<OrganizationInfo, InterfaceSettings>(
									(organization, serviceProvider) => 
										(serviceProvider.GetService(typeof(SettingsStore)) as SettingsStore).GetOrgInterfaceSettings(organization.Identifier)
								)
						);

		}

		[Fact]
		[Order(1)]
		public void SubsequentEntityRegistrationWithSettingsInheritanceShouldSucceed()
		{
			IUnifier unifier = context.Unifier;
			
			unifier.AddEntity<OrganizationUnitInfo>("Department")
					.With(
							new Settings<OrganizationUnitInfo, OrgUnitInterfaceSettings>(
										(department, serviceProvider) => 
											(serviceProvider.GetService(typeof(SettingsStore)) as SettingsStore).GetOrgUnitInterfaceSettings(department.Identifier)
									)
								.BasedOn<OrganizationInfo, InterfaceSettings>(
										(department, serviceProvider) =>
											new OrganizationInfo(department.Organization, string.Empty),
										true,
										(settings, orgSettings, serviceProvider) =>
										{
											settings.Layout = string.IsNullOrEmpty(settings.Layout)? orgSettings.Layout:settings.Layout;
											settings.Theme =
												string.IsNullOrEmpty(settings.Theme)
													? orgSettings.Theme
													: settings.Theme;

											return settings;
										}
									)
						);
		}

		[Fact]
		[Order(2)]
		public void EntityRegistrationWithDoubleInheritanceSettingShouldSucceed()
		{
			IUnifier unifier = context.Unifier;
			
			unifier.AddEntity<UserInfo>()
					.With<UserInterfaceSettings>(
							(user, serviceProvider) => null,
							(settings) =>
							{
								settings
									.BasedOn<OrganizationUnitInfo, OrgUnitInterfaceSettings>(
											"Department",
											(user, provider) =>
												new OrganizationUnitInfo(user.Department, string.Empty),
											false,
											(userSettings, orgSettings, serviceProvider) =>
											{
												userSettings.Avatar =
													string.IsNullOrEmpty(userSettings.Avatar)
														? orgSettings.Logo
														: userSettings.Avatar;
												
												userSettings.Layout =
													string.IsNullOrEmpty(userSettings.Layout)
														? orgSettings.Layout
														: userSettings.Layout;
											
												userSettings.Theme =
													string.IsNullOrEmpty(userSettings.Theme)
														? orgSettings.Theme
														: userSettings.Theme;

												return userSettings;
											}
										)
									.BasedOn<OrganizationInfo, InterfaceSettings>(
											(user, provider) =>
												new OrganizationInfo(user.Organization, string.Empty),
											(organization, serviceProvider) =>
											{
												return (serviceProvider.GetService(typeof(SettingsStore)) as SettingsStore)
													.GetOrgInterfaceSettings(organization.Identifier);
											},
											(userSettings, orgSettings, serviceProvider) =>
											{
												userSettings.LandingPage =
													string.IsNullOrEmpty(userSettings.LandingPage)
														? orgSettings.LandingPage
														: userSettings.LandingPage;
												
												return userSettings;
											}
										);
							}
						);
		}

		[Fact]
		[Order(3)]
		public void UnifierBuildShouldSucceed()
		{
			context.Unified = context.Unifier.Unify();
		}
		/*
		[Theory]
		[Order(60)]
		[InlineData("FirstType", "FirstEntity", "Purpose1", "Setting5")]
		[InlineData("FirstType", "FirstEntity", "Purpose2", "Setting1")]
		public void ShouldErrorIfUnknownSettingRequested(string type, string identifier, string purpose, string name)
		{
			Assert.Throws<UnknownSetting>(
					() => context.SettingsResolver.ResolveSetting(
						new EntityIdentifier {Identifier = identifier, Type = type},
						purpose,
						name)
				);
		}*/

	}
}