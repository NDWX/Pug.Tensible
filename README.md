# Pug.Settings

Simplify settings management and consumption in your application by declaring rules and intentions around settings within your application.

### Settings Schema
Rules and intentions are declared by building a settings schema. Settings are organized by **EntityType**, e.g. User, and further grouped into **Purpose** within each **EntityType**.

In the example below we declare a *UserInterface* purpose and a *User* entity type which has general *DisplayName* setting and three *UserInterface* related settings.

```c#
  ISchemaBuilder schemaBuilder = new SchemaBuilder();
  
  builder.RegisterPurpose(name: "UserInterface", description: "User interface settings");
    .RegisterEntityType(
      name: "User", description: "User entity settings",
      purposes: new Dictionary<string, IEnumerable<SettingDefinition>>
      {
        [string.Empty] = new []
        {
            new SettingDefinition(name: "DisplayName", description: "User display name", hasDefaultValue: false)
        }
        ["UserInterface"] = new []
        {
          new SettingDefinition(name: "Theme", description: "Colour theme for user interface", hasDefaultValue: true, defaultValue: "Light"),
          new SettingDefinition("LayoutDensity", "Spacing of UI elements", true, "Comfortable"),
          new SettingDefinition("DisplayGravatar", "Gravatar display flag", false)
        }
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
The example above will return setting value for *user* 'John' if exists in *settingStore*, otherwise the default value 'Light'.

### Planned Features
- User specified setting _default_ value
- Settings inheritance by **Purpose**
- Reference implementations of **ISettingsStore**
- Schema definition through configuration file

### Feedback
Report bugs or feature requests by creating new [issue](https://github.com/NDWX/Pug.Settings/issues).