using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Pug.Settings.Annotations;
using Settings.Schema;

namespace Pug.Settings
{
	public class Purpose<TPurpose> : IPurposeDefinition 
		where TPurpose : class, ISettingsPurpose, new()
	{
		private readonly Type SettingType = typeof(Setting<>);

		Dictionary<string, ISettingDefinition> settings = new Dictionary<string, ISettingDefinition>();

		public string Name { get; }
		public string ParentEntityType { get; }

		public Purpose(string name, string parentEntityType = null,
								PurposeSettingsInheritanceType parentSettingsInheritance =
									PurposeSettingsInheritanceType.Unspecified,
								PurposeSettingsInheritanceType settingsInheritance =
									PurposeSettingsInheritanceType.Unspecified)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));

			RunType = typeof(TPurpose);

			ParentEntityType = parentEntityType;

			ParentSettingsInheritance = parentSettingsInheritance;
			SettingsInheritance = settingsInheritance;

			PurposeAttribute purposeAttribute = RunType.GetCustomAttribute(typeof(PurposeAttribute)) as PurposeAttribute;

			if(purposeAttribute != null)
			{
				if(ParentSettingsInheritance == PurposeSettingsInheritanceType.Unspecified &&
					purposeAttribute.ParentSettingsInheritance != PurposeSettingsInheritanceType.Unspecified)
					ParentSettingsInheritance = purposeAttribute.ParentSettingsInheritance;

				if(SettingsInheritance == PurposeSettingsInheritanceType.Unspecified &&
					purposeAttribute.Inheritability != PurposeSettingsInheritanceType.Unspecified)
					SettingsInheritance = purposeAttribute.Inheritability;
			}

			MemberInfo[] properties = RunType.FindMembers(MemberTypes.Property, 
														BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy,
														(info, criteria) => true,
														null);

			// Find and validate possible settings
			foreach(MemberInfo memberInfo in properties)
			{
				ValidateAndRegisterSetting(memberInfo);
			}
			
			Settings = new ReadOnlyDictionary<string, ISettingDefinition>(settings);
		}

		private void ValidateAndRegisterSetting(MemberInfo settingMemberInfo)
		{
			PropertyInfo property = settingMemberInfo as PropertyInfo;

			Type propertyType = property.PropertyType;

			// check if property is annotated with Setting attribute
			SettingAttribute settingAttribute = property.GetCustomAttribute<SettingAttribute>(true);

			// check if property returns instance of Setting<>
			bool propertyReturnsSetting = propertyType.GetGenericTypeDefinition() == SettingType;

			// if property has been marked as a setting, 
			if(settingAttribute != null)
			{
				//but does not return Setting<>
				if(!propertyReturnsSetting)
					throw new SettingException(property.Name,
												$"Property has been marked as setting but return type does not inherit {SettingType.FullName}.");

				ResolveAndRegisterSettingDefinition(property, settingAttribute);
			}
			// if property has not been marked as setting but returns Setting<>
			else if(propertyReturnsSetting)
			{
				ResolveAndRegisterSettingDefinition(property, null);
			}
			// if property is not marked as setting nd does not return Settings<>.
			else
			{
				// Code is here for clarity, not necessity
				// ReSharper disable RedundantJumpStatement
				return;
				// ReSharper restore RedundantJumpStatement
			}
		}

		private void ResolveAndRegisterSettingDefinition(PropertyInfo declaration, SettingAttribute settingAttribute)
		{
			ISettingDefinition settingDefinition =
				ResolveSettingDefinition(declaration, settingAttribute);

			settings.Add(declaration.Name, settingDefinition);
		}

		private SettingDefinition ResolveSettingDefinition(PropertyInfo declaration, SettingAttribute settingAttribute)
		{
			Type propertyType = declaration.PropertyType;
			Type settingValueType = propertyType.GetGenericArguments().First();

			switch(settingValueType)
			{
				// setting value must be of primitive type,
				case var type when !type.IsPrimitive:
					throw new SettingException(declaration.Name, "Only primitives may be used as setting value type.");
				// but cannot be pointer type
				case var type when type == typeof(IntPtr) || type == typeof(UIntPtr):
					throw new SettingException(declaration.Name, "Setting value cannot be pointer.");
			}

			if(settingAttribute != null)
			{
				if(settingAttribute.HasDefaultValue && settingAttribute.DefaultValue != null)
				{
					Type defaultValueType = settingAttribute.DefaultValue.GetType();

					if( !defaultValueType.InheritsOrEqualsTo(settingValueType) )
						throw new SettingException(declaration.Name, $"Default value of type '{settingValueType.FullName}' is expected, but type '{defaultValueType.FullName}' is found.");
				}
				
				return new SettingDefinition(declaration, settingAttribute.HasDefaultValue,
											settingAttribute.DefaultValue,
											settingAttribute.ParentInheritanceType == PurposeSettingsInheritanceType.Unspecified
												? ParentSettingsInheritance
												: settingAttribute.ParentInheritanceType,
											settingAttribute.Inheritability == PurposeSettingsInheritanceType.Unspecified
												? SettingsInheritance
												: settingAttribute.Inheritability
					);
			}
			else
			{
				return new SettingDefinition(declaration, settingAttribute.HasDefaultValue,
											settingAttribute.DefaultValue,
											ParentSettingsInheritance,
											SettingsInheritance
					);
			}
		}

		public PurposeSettingsInheritanceType ParentSettingsInheritance { get; }
		
		public PurposeSettingsInheritanceType SettingsInheritance { get; }
		
		public Type RunType { get; }
		
		public IDictionary<string, ISettingDefinition> Settings { get; }

		public ISettingDefinition this[string name]
		{
			get
			{
				if(settings.ContainsKey(name))
					return settings[name];

				return null;
			}
		}
	}
}