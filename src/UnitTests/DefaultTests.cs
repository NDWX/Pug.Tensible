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
		
		[Fact]
		public void FirstPurposeRegistrationShouldSucceed()
		{
			context.Builder.RegisterPurpose("Purpose1", "Description of purpose1");
		}

		[Fact]
		[Order(1)]
		public void SubsequentPurposeRegistrationShouldSucceed()
		{
			context.Builder.RegisterPurpose("Purpose2", "Description of purpose2");
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
		[Order(10)]
		public void EntityRegistrationWithExistingPurposeShouldSucceed()
		{
			context.Builder.RegisterEntityType(
					"FirstType", "Description of Entity 1",
					purposes: new[]
					{
						new EntityPurposeDefinition(
								"Purpose1", null, null, new[]
								{
									new SettingDefinition(
										"Setting1", "Description of setting 1", true,
										"s1DefaultValue"),
									new SettingDefinition(
										"Setting2", "Description of setting 2", true,
										"NewDefaultValue"),
									new SettingDefinition(
										"Setting3", "Description of setting 3", false)
								},
								new PurposeSettingsInheritance(
									PurposeSettingsInheritanceType.Inherit,
									new []{"Setting1", "Setting2"})
							)
					}
				);
		}

		[Fact]
		[Order(11)]
		public void EntityRegistrationWithDuplicateNameShouldFail()
		{
			Assert.Throws<DuplicateNameException>(
					() =>
					{
						context.Builder.RegisterEntityType(
								"FirstType", "Description of Entity 2",
								purposes: new[]
								{
									new EntityPurposeDefinition(
											"Purpose2", null, null,
											new[]
											{
												new SettingDefinition(
													"Setting1", "Description of setting 1",
													true, "sValue")
											},
											new PurposeSettingsInheritance(
												PurposeSettingsInheritanceType.Inherit,
												null)
										)
								}
							);
					}
				);
		}

		[Fact]
		[Order(12)]
		public void EntityRegistrationWithExistingDuplicatedPurposeNameShouldFail()
		{
			Assert.Throws<DuplicateNameException>(
			() => context.Builder.RegisterEntityType(
					"SecondType", "Description of Entity 2",
					purposes: new[]
					{
						new EntityPurposeDefinition(
								"Purpose1", null, null, new[]
								{
									new SettingDefinition(
										"Setting1", "Description of setting 1", 
										true, "s1DefaultValue"),
									new SettingDefinition(
										"Setting2", "Description of setting 2", true,
										"NewDefaultValue"),
									new SettingDefinition(
										"Setting3", "Description of setting 3", false)
								},
								new PurposeSettingsInheritance(
									PurposeSettingsInheritanceType.Inherit,
									null)
							),
						new EntityPurposeDefinition(
								"Purpose1", null, null, new[]
								{
									new SettingDefinition(
										"Setting1", "Description of setting 1",
										true, "s1DefaultValue")
								},
								new PurposeSettingsInheritance(
									PurposeSettingsInheritanceType.Inherit,
									null)
							)
					}
				)
			);
		}

		[Fact]
		[Order(13)]
		public void EntityRegistrationWithoutExistingPurposeShouldFail()
		{
			Assert.Throws<UnknownPurpose>(
				() =>
				{
					context.Builder.RegisterEntityType(
							"SecondType", "Description of Entity 2",
							purposes: new[]
							{
								new EntityPurposeDefinition(
										"Purpose3", null, null,
										new[]
										{
											new SettingDefinition(
												"Setting1", "Description of setting 1", true, "sValue")
										},
										new PurposeSettingsInheritance(
												PurposeSettingsInheritanceType.Inherit,
												null
											)
									)
							}
						);
				}
			);
		}

		[Fact]
		[Order(14)]
		public void EntityRegistrationWithExistingDuplicatedPurposeSettingNameShouldFail()
		{
			Assert.Throws<DuplicateNameException>(
					() => context.Builder.RegisterEntityType(
							"SecondType", "Description of Entity 2",
							purposes: new[]
							{
								new EntityPurposeDefinition(
										"Purpose1", null, null, new[]
										{
											new SettingDefinition(
												"Setting1", "Description of setting 1", 
												true, "s1DefaultValue"),
											new SettingDefinition(
												"Setting1", "Description of setting 2", true,
												"NewDefaultValue"),
											new SettingDefinition(
												"Setting3", "Description of setting 3", false)
										},
										new PurposeSettingsInheritance(
											PurposeSettingsInheritanceType.Inherit,
											null)
									)
							}
						)
				);
		}

		[Fact]
		[Order(15)]
		public void InheritanceofNonExistingParentSettingShouldFail()
		{
			Assert.Throws<UnknownSetting>(
				() => context.Builder.RegisterEntityType(
					"SecondType", "Description of Entity 2",
					purposes: new[]
					{
						new EntityPurposeDefinition(
							"Purpose1", "FirstType",
							inheritance: new PurposeSettingsInheritance(
								PurposeSettingsInheritanceType.Inherit, new[] {"Setting4"}),
							new[]
							{
								new SettingDefinition(
									"Setting1", "Description of setting 1", true, "t2s1Value")
							},
							new PurposeSettingsInheritance(
									PurposeSettingsInheritanceType.Inherit,
									null
							)
						)
					}

				)
			);
		}

		[Fact]
		[Order(16)]
		public void InheritanceOfNonInheritableSettingShouldFail()
		{
			Assert.Throws<NotInheritable>(
					() => context.Builder.RegisterEntityType(
							"SecondType", "Description of Entity 2",
							purposes: new[]
							{
								new EntityPurposeDefinition(
										"Purpose1", "FirstType",
										inheritance: new PurposeSettingsInheritance(
											PurposeSettingsInheritanceType.Inherit, new[] {"Setting3"}),
										new[]
										{
											new SettingDefinition(
												"Setting1", "Description of setting 1", true, "t2s1Value")
										},
										new PurposeSettingsInheritance(
											PurposeSettingsInheritanceType.Inherit,
											null
										)
									)
							}

						)
				);
		}

		[Fact]
		[Order(17)]
		public void SubsequentProperEntityTypeRegistrationShouldSucceed()
		{
			// implicit inheritance
			context.Builder.RegisterEntityType(
					"SecondType", "Description of Entity 2",
					purposes: new[]
					{
						new EntityPurposeDefinition(
								"Purpose1", "FirstType", null,
								new[]
								{
									new SettingDefinition(
										"Setting1", "Description of setting 1", true, "t2p1s1Value")
								},
								new PurposeSettingsInheritance(
										PurposeSettingsInheritanceType.Inherit,
										null
									)
							),
						
						new EntityPurposeDefinition(
								"Purpose2", null, null,
								new[]
								{
									new SettingDefinition(
										"Setting1", "Description of setting 1", true, "t2s1Value"),
									new SettingDefinition(
									"Setting2", "Description of setting 2", true, "t2p2s2Value")
								},
								new PurposeSettingsInheritance(
										PurposeSettingsInheritanceType.Inherit,
										null
									)
							)
					}
				);
			
			// explicit inheritance
			context.Builder.RegisterEntityType(
					"ThirdType", "Description of Entity 3",
					purposes: new[]
					{
						new EntityPurposeDefinition(
								"Purpose2", null, new PurposeSettingsInheritance(PurposeSettingsInheritanceType.Inherit, new []{"Setting1"}), null,
								new PurposeSettingsInheritance(
										PurposeSettingsInheritanceType.DoNotInherit,
										null
									)
							)
					}
				);
		}

		[Fact]
		[Order(20)]
		public void SchemaBuildShouldNotError()
		{
			context.Schema = context.Builder.Build();

			context.Resolver = new Resolver(context.Schema, new DummySettingStore(), new DummyEntityRelationshipResolver());

			Assert.NotNull(context.Schema);
		}

		[Fact]
		[Order(21)]
		public void InheritableSettingShouldPropagateToInheritor()
		{
			
		}

		[Fact]
		[Order(22)]
		public void NonInheritableSettingShouldPropagateToInheritor()
		{
			
		}

		[Fact]
		[Order(23)]
		public void InheritanceBlockShouldBeHonoured()
		{
			
		}

		[Fact]
		[Order(24)]
		public void SettingOverrideShouldBeHonoured()
		{
			
		}

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
	}
}