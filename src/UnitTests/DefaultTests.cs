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
										"Setting3", "Description of setting 3", false),
									new SettingDefinition(
									"Setting4", "Description of setting 4", false)
								},
								new PurposeSettingsInheritance(
									PurposeSettingsInheritanceType.Inherit,
									new []{"Setting1", "Setting2", "Setting3"})
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
								PurposeSettingsInheritanceType.Inherit, new[] {"Setting5"}),
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
		[Order(17)]
		public void SubsequentProperEntityTypeRegistrationShouldSucceed()
		{
			// implicit inheritance
			context.Builder.RegisterEntityType(
					"SecondType", "Description of Entity 2",
					purposes: new[]
					{
						new EntityPurposeDefinition(
								"Purpose1", "FirstType", 
								inheritance: new PurposeSettingsInheritance(PurposeSettingsInheritanceType.DoNotInherit, 
																						new string[] {"Setting2"}), 
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
								"Purpose1", "SecondType", new PurposeSettingsInheritance(PurposeSettingsInheritanceType.Inherit, new []{"Setting1", "Setting3"}), null,
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

			context.SettingsResolver = context.Schema.GetResolver(new DummySettingStore(), new DummyEntityRelationshipResolver());

			Assert.NotNull(context.Schema);
		}

		[Fact]
		[Order(21)]
		public void InheritableSettingShouldPropagateToInheritor()
		{
			IEntityTypeSchema entityTypeSchema = context.Schema.GetEntityType("SecondType");
			
			Assert.NotNull(entityTypeSchema);

			IEntityPurposeSchema purposeSchema = entityTypeSchema.GetPurpose("Purpose1");
			
			Assert.NotNull(purposeSchema);

			Assert.True(purposeSchema.Settings.ContainsKey("Setting3"));

			ISettingSchema settingSchema = purposeSchema.Settings["Setting3"];
			
			Assert.True(settingSchema.Source.Type == DefinitionSourceType.ParentEntityType);
		
			Assert.True(settingSchema.Source.EntityType == purposeSchema.Definition.ParentEntityType);
		}

		[Fact]
		[Order(22)]
		public void NonInheritableSettingShouldNotPropagateToInheritor()
		{
			IEntityTypeSchema entityTypeSchema = context.Schema.GetEntityType("SecondType");
			
			Assert.NotNull(entityTypeSchema);

			IEntityPurposeSchema purposeSchema = entityTypeSchema.GetPurpose("Purpose1");
			
			Assert.NotNull(purposeSchema);

			Assert.False(purposeSchema.Settings.ContainsKey("Setting4"));
		}

		[Fact]
		[Order(23)]
		public void InheritanceBlockShouldBeHonoured()
		{
			IEntityTypeSchema entityTypeSchema = context.Schema.GetEntityType("SecondType");
			
			Assert.NotNull(entityTypeSchema);

			IEntityPurposeSchema purposeSchema = entityTypeSchema.GetPurpose("Purpose1");
			
			Assert.NotNull(purposeSchema);

			Assert.False(purposeSchema.Settings.ContainsKey("Setting2"));
		}

		[Fact]
		[Order(24)]
		public void InheritanceDirectiveShouldBeHonoured()
		{
			IEntityTypeSchema entityTypeSchema = context.Schema.GetEntityType("ThirdType");
			
			Assert.NotNull(entityTypeSchema);

			IEntityPurposeSchema purposeSchema = entityTypeSchema.GetPurpose("Purpose1");
			
			Assert.NotNull(purposeSchema);

			Assert.True(purposeSchema.Settings.ContainsKey("Setting1"));

			ISettingSchema settingSchema = purposeSchema.Settings["Setting1"];
			
			Assert.True(settingSchema.Source.Type == DefinitionSourceType.ParentEntityType);
		
			Assert.True(settingSchema.Source.EntityType == purposeSchema.Definition.ParentEntityType);
			
			Assert.True(settingSchema.Definition.DefaultValue == "t2p1s1Value");
			
			Assert.False(purposeSchema.Settings.ContainsKey("Setting2"));
		}

		[Fact]
		[Order(25)]
		public void SecondLevelInheritanceShouldWork()
		{
			IEntityTypeSchema entityTypeSchema = context.Schema.GetEntityType("ThirdType");
			
			Assert.NotNull(entityTypeSchema);

			IEntityPurposeSchema purposeSchema = entityTypeSchema.GetPurpose("Purpose1");
			
			Assert.NotNull(purposeSchema);

			Assert.True(purposeSchema.Settings.ContainsKey("Setting3"));

			ISettingSchema settingSchema = purposeSchema.Settings["Setting3"];
			
			Assert.True(settingSchema.Source.Type == DefinitionSourceType.ParentEntityType);
		
			Assert.True(settingSchema.Source.EntityType == "SecondType");
			
			Assert.True(settingSchema.Source.Source.Type == DefinitionSourceType.ParentEntityType);
		
			Assert.True(settingSchema.Source.Source.EntityType == "FirstType");
		}

		[Fact]
		[Order(26)]
		public void SettingOverrideShouldBeHonoured()
		{
			IEntityTypeSchema entityTypeSchema = context.Schema.GetEntityType("SecondType");
			
			Assert.NotNull(entityTypeSchema);

			IEntityPurposeSchema purposeSchema = entityTypeSchema.GetPurpose("Purpose1");
			
			Assert.NotNull(purposeSchema);
			
			Assert.True(purposeSchema.Settings.ContainsKey("Setting1"));

			ISettingSchema settingSchema = purposeSchema.Settings["Setting1"];
			
			Assert.True(settingSchema.Source.Type == DefinitionSourceType.EntityType);
		
			Assert.True(settingSchema.Source.EntityType == entityTypeSchema.GetEntityTypeInfo().Name);
			
			Assert.True(settingSchema.Definition.HasDefaultValue == true && settingSchema.Definition.DefaultValue == "t2p1s1Value" );
		}

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
		}

		[Fact]
		[Order(60)]
		public void ShouldGetNullIfSettingNotStoredAndNoDefaultSpecified()
		{
			Setting setting = context.SettingsResolver.ResolveSetting(
				new EntityIdentifier {Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1",
				"Setting4");

			Assert.Null(setting);
		}

		[Fact]
		[Order(60)]
		public void ShouldReturnSettingDefaultIfNotExistInStore()
		{
			Setting setting = context.SettingsResolver.ResolveSetting(
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
			Setting setting = context.SettingsResolver.ResolveSetting(
				new EntityIdentifier {Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1",
				"Setting1");

			Assert.NotNull(setting);
			Assert.Equal(setting.ValueSource.Type, SettingValueSourceType.User);
			Assert.Equal("Test Value", setting.Value);
		}

		[Fact]
		[Order(60)]
		public void PurposeLevelSettingsResolutionBehaviourShouldBeConsistentWithSettingLevelResolution()
		{
			IDictionary<string, Setting> settings = context.SettingsResolver.ResolveSettings(
				new EntityIdentifier
					{Identifier = "FirstEntity", Type = "FirstType"},
				"Purpose1");

			Assert.True(settings.ContainsKey("Setting1"));
			Setting setting = settings["Setting1"];
			Assert.Equal(SettingValueSourceType.User, setting.ValueSource.Type );
			Assert.Equal("Test Value", setting.Value);
			
			Assert.True(settings.ContainsKey("Setting2"));
			setting = settings["Setting2"];
			Assert.Equal(SettingValueSourceType.Default, setting.ValueSource.Type);
			Assert.Equal("NewDefaultValue", setting.Value);
		}

		[Fact]
		[Order(61)]
		public void DefaultValueInheritanceShouldWork()
		{
			Setting setting = context.SettingsResolver.ResolveSetting(
				new EntityIdentifier
					{Identifier = "ThirdEntity", Type = "ThirdType"},
				"Purpose1", "Setting1");
			
			Assert.Equal("t2p1s1Value", setting.Value);
			Assert.Equal(SettingValueSourceType.Parent | SettingValueSourceType.Default, setting.ValueSource.Type);
			Assert.Equal("SecondType", setting.ValueSource.Entity.Type);
		}

		[Fact]
		[Order(61)]
		public void SettingValueInheritanceShouldWork()
		{
			Setting setting = context.SettingsResolver.ResolveSetting(
				new EntityIdentifier
					{Identifier = "ThirdEntity", Type = "ThirdType"},
				"Purpose1", "Setting3");
			
			Assert.Equal("E1P1S3Value", setting.Value);
			Assert.Equal(SettingValueSourceType.Parent | SettingValueSourceType.User, setting.ValueSource.Type);
			Assert.Equal("SecondType", setting.ValueSource.Entity.Type);
			Assert.Equal("SecondEntity", setting.ValueSource.Entity.Identifier);
			Assert.Equal(SettingValueSourceType.Parent | SettingValueSourceType.User, setting.ValueSource.Source.Type);
			Assert.Equal("FirstType", setting.ValueSource.Source.Entity.Type);
			Assert.Equal("FirstEntity", setting.ValueSource.Source.Entity.Identifier);
		}
	}
}