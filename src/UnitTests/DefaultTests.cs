using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Settings;
using Settings.Schema;
using Xunit;
using Xunit.Extensions.Ordering;
// ReSharper disable HeapView.ClosureAllocation

namespace UnitTests
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

		[Theory]
		[Order(60)]
		[InlineData("FirstType", "FirstEntity", "Purpose1", "Setting4")]
		[InlineData("FirstType", "FirstEntity", "Purpose2", "Setting1")]
		public void ShouldErrorIfUnknownSettingRequested(string type, string identifier, string purpose, string name)
		{
			Assert.Throws<UnknownSetting>(
					() => context.Resolver.ResolveSetting(
						new EntityIdentifier {Identifier = identifier, Type = type},
						purpose,
						name)
				);
		}

		[Fact]
		[Order(2)]
		public void DuplicatePurposeRegistrationShouldFail()
		{
			Assert.Throws<DuplicateNameException>(
					() => context.Builder.RegisterPurpose("Purpose2", "Description of purpose2")
				);
		}

		[Fact]
		[Order(20)]
		public void EntityRegistrationWithDuplicateNameShouldFail()
		{
			Assert.Throws<DuplicateNameException>(
					() =>
					{
						context.Builder.RegisterEntityType(
								"FirstType", "Description of Entity 2",
								new Dictionary<string, IEnumerable<SettingDefinition>>
								{
									["Purpose2"] = new[]
									{
										new SettingDefinition(
											"Setting1", "Description of setting 1", true, "sValue")
									}
								}
							);
					}
				);
		}

		[Fact]
		[Order(10)]
		public void EntityRegistrationWithExistingPurposeShouldSucceed()
		{
			context.Builder.RegisterEntityType(
					"FirstType", "Description of Entity 1",
					new Dictionary<string, IEnumerable<SettingDefinition>>
					{
						["Purpose1"] = new[]
						{
							new SettingDefinition("Setting1", "Description of setting 1", true,
												"s1DefaultValue"),
							new SettingDefinition("Setting2", "Description of setting 2", true,
												"NewDefaultValue"),
							new SettingDefinition("Setting3", "Description of setting 3", false)
						}
					}
				);
		}

		[Fact]
		[Order(30)]
		public void EntityRegistrationWithoutExistingPurposeShouldFail()
		{
			Assert.Throws<UnknownPurpose>(
					() =>
					{
						context.Builder.RegisterEntityType(
								"SecondType", "Description of Entity 2",
								new Dictionary<string, IEnumerable<SettingDefinition>>
								{
									["Purpose3"] = new[]
									{
										new SettingDefinition(
											"Setting1", "Description of setting 1", true, "sValue")
									}
								}
							);
					}
				);
		}

		[Fact]
		public void FirstPurposeRegistrationShouldSucceed()
		{
			context.Builder.RegisterPurpose("Purpose1", "Description of purpose1");
		}

		[Fact]
		[Order(50)]
		public void SchemaBuildShouldNotError()
		{
			context.Schema = context.Builder.Build();

			context.Resolver = new Resolver(context.Schema, new DummySettingStore());

			Assert.NotNull(context.Schema);
		}

		[Fact]
		[Order(60)]
		public void ShouldGetNewDefaultSettingValue()
		{
			Setting setting = context.Resolver.ResolveSetting(
				new EntityIdentifier {Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1",
				"Setting2");

			Assert.NotNull(setting);
			Assert.Equal(setting.ValueSource.Type, SettingValueSourceType.Default);
			Assert.Equal("NewDefaultValue", setting.Value);
		}

		[Fact]
		[Order(60)]
		public void ShouldGetNullIfSettingNotStoredAndNoDefaultSpecified()
		{
			Setting setting = context.Resolver.ResolveSetting(
				new EntityIdentifier {Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1",
				"Setting3");

			Assert.Null(setting);
		}

		[Fact]
		[Order(60)]
		public void ShouldReturnSettingDefaultIfNotExistInStore()
		{
			Setting setting = context.Resolver.ResolveSetting(
				new EntityIdentifier {Identifier = "SecondEntity", Type = "FirstType"},
				"Purpose1",
				"Setting1");

			Assert.NotNull(setting);
			Assert.Equal(setting.ValueSource.Type, SettingValueSourceType.Default);
			Assert.Equal(setting.Value, "s1DefaultValue");
		}

		[Fact]
		[Order(60)]
		public void ShouldReturnSpecifiedSettingFromStoreIfExists()
		{
			Setting setting = context.Resolver.ResolveSetting(
				new EntityIdentifier {Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1",
				"Setting1");

			Assert.NotNull(setting);
			Assert.Equal(setting.ValueSource.Type, SettingValueSourceType.User);
			Assert.Equal(setting.Value, "Test Value");
		}

		[Fact]
		[Order(60)]
		public void PurposeLevelSettingsResolutionBehaviousShouldBeConsistentWithSettingLevelResolution()
		{
			IDictionary<string, Setting> settings = context.Resolver.ResolveSettings(
				new EntityIdentifier
					{Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1");

			Assert.True(settings.ContainsKey("Setting1"));
			Setting setting = settings["Setting1"];
			Assert.Equal(setting.ValueSource.Type, SettingValueSourceType.User);
			Assert.Equal(setting.Value, "Test Value");
			
			Assert.True(settings.ContainsKey("Setting2"));
			setting = settings["Setting2"];
			Assert.Equal(setting.ValueSource.Type, SettingValueSourceType.Default);
			Assert.Equal("NewDefaultValue", setting.Value);
		}

		[Fact]
		[Order(40)]
		public void SubsequentProperEntityTypeRegistrationShouldSucceed()
		{
			context.Builder.RegisterEntityType(
					"SecondType", "Description of Entity 2",
					new Dictionary<string, IEnumerable<SettingDefinition>>
					{
						["Purpose2"] = new[]
						{
							new SettingDefinition(
								"Setting1", "Description of setting 1", true, "t2s1Value")
						}
					}
				);
		}

		[Fact]
		[Order(1)]
		public void SubsequentPurposeRegistrationShouldSucceed()
		{
			context.Builder.RegisterPurpose("Purpose2", "Description of purpose2");
		}
	}
}