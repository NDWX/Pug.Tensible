# Pug.Settings

Simplify settings management and consumption in your application by declaring rules and intentions around settings within your application.

### Settings Schema
Rules and intentions are declared by building a settings schema. Settings are organized by **EntityType**, e.g. User, and further grouped into **Purpose** within each **EntityType**.

In the example below we declare a *UserInterface* purpose and an *Organization* entity type which has general *DisplayName* setting and three *UserInterface* related settings.

```c#
  ISchemaBuilder schemaBuilder = new SchemaBuilder();
  
  builder.RegisterPurpose(name: "UserInterface", description: "User interface settings");
    .RegisterEntityType(
      name: "Organization", description: "Organization entity settings",
      purposes: new[]
      {
        new EntityPurposeDefinition(
          name: "", parentEntityType: null, inheritance: null, 
          settings: new[]
          {
             new SettingDefinition(name: "DisplayName", description: "Organization display name", hasDefaultValue: false)
          }
        ),
        new EntityPurposeDefinition(
          name: "UserInterface", parentEntityType: null, inheritance: null, 
          settings: new[]
          {
            new SettingDefinition(name: "Theme", description: "Colour theme for user interface", hasDefaultValue: true, defaultValue: "Light"),
            new SettingDefinition("LayoutDensity", "Spacing of UI elements", true, "Comfortable"),
            new SettingDefinition("DisplayGravatar", "Gravatar display flag", false),
            new SettingDefinition("Logo", "Company logo", false)
          }
        )
      }
    );
    
  ISettingsSchema settingsSchema = builder.Build();
```
**Purpose** and **EntityType** names must be a not null and non-empty string, but an entity type may have general settings declared within a purpose with empty name.

### Current Features

##### Single Source of Truth
Declare *settings* in advance in one single source of truth declaration and identify use of non-existing or obsolete *setting* during testing.

##### Setting *Default* Resolution
Easily manage setting default value by specifying default value in setting definition. Setting resolver will return *default* value if no user specified value is found.

##### Settings Inheritance
Avoid repetitive settings declaration that can be inherited by one entity type from another.

Settings of an entity type (parent) may be inherited by another entity type (child) when the child entity type declares a purpose of the same name as that in the parent entity type and specifying the parent entity type name as the *parent*. In short, settings are not inherited at entity type level like class/type members inheritance as in most programming languages, but rather at *purpose* level.

When a child entity type *purpose* inherits settings from a parent entity type, it inherits the setting definition. An instance of the child entity type may also inherit setting value from relevant instance of the parent entity type.

Consider the code below which builds on entity settings registration example above:
```c#
builder.RegisterEntityType(
    name: "User", Description: "User entity type",
    purposes: new []
    {
        new EntityPurposeDefinition(
            name: "", 
            // 'User' entity type will inherit settings from 'Organization' entity type for this purpose
            parentEntityType: "Organization", 
            // No explicit inheritance inclusion or exclusion, meaning all inheritable settings will be inherited
            inheritance: null, 
            // 'User' entity type level setting declaration
            settings: new []
            {
                new SettingDefinition("EmailAddress", "User email address")
            },
            // No setting may be further inherited by other entity types 
            inheritability: new PurposeSettingsInheritance(
                PurposeSettingsInheritanceType.DoNotInherit
            )
        ),
        new EntityPurposeDefinition(
            name: "UserInterface",
            // 'User' entity type will inherit settings from 'Organization' entity type for UI purpose
            parentEntityType: "Organization",
            // Inherit all settings from 'Organization' entity type except 'Logo'
            inheritance: new PurposeSettingsInheritance(
                PurposeSettingsInheritanceType.DoNotInherit,
                applicableSettings: new [] {"Logo"}
            ),
            // No new setting is declared for UI purpose at 'User' level
            settings: null,
            // No setting may be further inherited by other entity types 
            inheritability: new PurposeSettingsInheritance(
                PurposeSettingsInheritanceType.DoNotInherit
            )
        )
    }
);
    
// ISchemaBuilder.Build() function is called again for sake of clarity. 
// This function may be called only once after all purposes and entity types have been registered
// Calling RegisterPurpose() or RegisterEntityType() after Build() has been called will throw InvalidOperationException.
ISettingsSchema settingsSchema = builder.Build();
```

Based on the exmple above, '*User*' entity type declares two purposes that inherits settings from '*Organization*' entity type.

Settings are inherited by purpose, which means an entity type purpose can only inherit settings of the same purpose from another entity type.

Following the concept of inheritance mentioned above, *User* _default_ purpose (purpose with empty name) will effectively have 'DisplayName' setting that is inherited from '*Organization*' entity type and 'EmailAddress' setting which is declared within '*User*' entity type itself, and '_UserInterface_' purpose of '*User*' entity type will inherit all settings declared within '_UserInterface_' purpose of '*Organization*'.

### Setting Resolution
Settings may be resolved as follow:
```c#
Resolver resolver = new Resolver(settingsSchema, settingStore: settingStore);

Setting setting = resolver.ResolveSetting(
    entity: new EntityIdentifier { Identifier = "John", Type = "User" },
    purpose: "UserInterface",
    name: "Theme"
);
```

The example above will return setting value for *user* 'John' if exists in *settingStore*, otherwise the default value 'Light' will be returned.

### Planned Features
- User specified setting _default_ value
- Reference implementations of **ISettingsStore**
- Schema definition through configuration file

### Feedback
Report bugs or feature requests by creating new [issue](https://github.com/NDWX/Pug.Settings/issues).