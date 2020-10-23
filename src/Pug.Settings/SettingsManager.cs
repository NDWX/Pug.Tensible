using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Pug.Settings.Schema;
using Settings;
using Settings.Schema;

namespace Pug.Settings
{
	public class SettingsManager : ISettingsManager
	{
		// readonly Type _settingValueSourceType = typeof(SettingValueSource);
		// readonly Type _settingMetadataType = typeof(ISettingMetaData);

		private readonly Dictionary<Type, Func<EntityIdentifier, string, object>> settingsResolvers = new Dictionary<Type, Func<EntityIdentifier, string, object>>();
		private readonly Dictionary<string, object> purposeSettingMutators = new Dictionary<string, object>();
		private readonly Dictionary<string, object> purposeSettingAccessors = new Dictionary<string, object>();
		private ISettingsStoreProvider _storeProvider;
		private readonly IEntityRelationshipResolver _entityRelationshipResolver;

		public ISettingsSchema Schema { get; }

		public SettingsManager(ISettingsSchema schema, ISettingsStoreProvider storeProvider,
								IEntityRelationshipResolver entityRelationshipResolver)
		{
			if(schema == null) throw new ArgumentNullException(nameof(schema));
			
			Schema = new ReadOnlySchema(schema);
			
			_storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
			_entityRelationshipResolver = entityRelationshipResolver ?? throw new ArgumentNullException(nameof(entityRelationshipResolver));
		}

		private void AssignSettingValue<TPurpose>(TPurpose purposeSettings, ISettingDefinition settingDefinition,
												object setting) 
			where TPurpose : class, ISettingsPurpose, new()
		{
			Action<TPurpose, object> action;

			Type purposeSettingsType = typeof(TPurpose);
			
			string purposeSettingMutatorIdentifier = $"{purposeSettingsType.FullName}.{settingDefinition.Declaration.Name}.{settingDefinition.Declaration.SetMethod.Name}, {purposeSettingsType.Assembly.FullName}";
			
			lock(purposeSettingMutators)
			{
				if(!purposeSettingMutators.ContainsKey(purposeSettingMutatorIdentifier))
					action = purposeSettingMutators[purposeSettingMutatorIdentifier] as Action<TPurpose, object>;
				else
				{
					ParameterExpression purposeExpression = Expression.Parameter(purposeSettingsType, "purpose");
					// ParameterExpression methodInfoExpression = Expression.Parameter(typeof(MethodInfo), "methodInfo");
					MemberExpression propertyExpression =
						Expression.Property(purposeExpression, settingDefinition.Declaration.Name);
					ParameterExpression valueExpression = Expression.Parameter(typeof(object), "value");

					Expression<Action<TPurpose, object>> setExpression =
						// ReSharper disable HeapView.ObjectAllocation
						Expression.Lambda<Action<TPurpose, object>>(
								Expression.Assign(
										propertyExpression,
										Expression.TypeAs(
												valueExpression,
												settingDefinition.Declaration.PropertyType
											)
									),
								purposeExpression, valueExpression
							);
					// ReSharper restore HeapView.ObjectAllocation

					action = setExpression.Compile();

					purposeSettingMutators[purposeSettingMutatorIdentifier] = action;
				}
			}

			action(purposeSettings, setting);
			
			/*
			settingDefinition.Declaration.SetMethod.Invoke(
					purposeSettings,
					new[]
					{
						setting
					}
				);*/
		}

		private ISetting GetSetting(Type purposeType, object purposeSettings, ISettingDefinition settingDefinition )
		{
			Func<object, object> settingAccessor;
			
			string purposeSettingAccessorIdentifier = $"{purposeType.FullName}.{settingDefinition.Declaration.Name}.{settingDefinition.Declaration.GetMethod.Name}, {purposeType.Assembly.FullName}";
			
			lock(purposeSettingAccessors)
			{
				if(!purposeSettingAccessors.ContainsKey(purposeSettingAccessorIdentifier))
					settingAccessor = purposeSettingAccessors[purposeSettingAccessorIdentifier] as Func<object, object>;
				else
				{
					ParameterExpression purposeParameterExpression = Expression.Parameter(typeof(object), "purpose");
					UnaryExpression purposeExpression = Expression.Unbox(purposeParameterExpression, purposeType);
					
					MemberExpression propertyExpression =
						Expression.Property(purposeExpression, settingDefinition.Declaration.Name);

					// ReSharper disable HeapView.ObjectAllocation
					Expression<Func<object, object>> getExpression =
						Expression.Lambda<Func<object, object>>(
								propertyExpression,
								purposeParameterExpression
							);
					// ReSharper restore HeapView.ObjectAllocation

					settingAccessor = getExpression.Compile();

					purposeSettingAccessors[purposeSettingAccessorIdentifier] = settingAccessor;
				}
			}

			return settingAccessor(purposeSettings) as ISetting;
		}

		private TPurpose GetSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose, SettingResolutionContext resolutionContext )
			where TPurpose : class, ISettingsPurpose, new()
		{
			IEntityTypeSettingsSchema entityTypeSchema;
			IPurposeDefinition purposeDefinition;
			
			if(resolutionContext == null)
			{
				entityTypeSchema = Schema[entityIdentifier.Type];
				purposeDefinition = entityTypeSchema[purpose];
			}
			else
			{
				entityTypeSchema = resolutionContext.EntityTypeSettingsSchema;
				purposeDefinition = resolutionContext.PurposeDefinition;
			}

			if(entityTypeSchema == null)
				throw new EntityTypeException(entityIdentifier.Type, $"Unable to find entity type '{entityIdentifier.Type}' in schema.");

			if(purposeDefinition == null)
				throw new PurposeException(
					purpose, $"Unable to find purpose '{purpose}' in entity type '{entityIdentifier.Type}' schema.");

			IDictionary<string, string> settingValues = _storeProvider.GetSettings(entityIdentifier, purpose);

			if(settingValues == null || settingValues.Count == 0)
				return null;

			TPurpose purposeSettings = new TPurpose();

			foreach(ISettingDefinition settingDefinition in purposeDefinition.Settings.Values)
			{
				object settingValue = null;
				SettingValueSource settingValueSource = null;

				if(settingValues.ContainsKey(settingDefinition.Name))
				{
					settingValue = settingValues[settingDefinition.Name]; // Deserialize(settingValues[settingDefinition.Name], settingDefinition.ValueType);
					settingValueSource = new SettingValueSource(SettingValueSourceType.User, entityIdentifier);
				}
				else
				{
					if(settingDefinition.HasDefaultValue)
					{
						settingValue = settingDefinition.DefaultValue;
						settingValueSource = new SettingValueSource(SettingValueSourceType.Default, entityIdentifier);
					}
					else if( resolutionContext?.ParentEntityPurposeSettings != null)
					{
						IPurposeDefinition parentPurposeDefinition = resolutionContext.ParentEntityTypeSettingsSchema[purpose];

						if(parentPurposeDefinition.Settings.TryGetValue(
							settingDefinition.Name, out ISettingDefinition parentSettingDefinition))
						{
							ISetting parentSetting = GetSetting(parentPurposeDefinition.RunType,
																resolutionContext.ParentEntityPurposeSettings,
																parentSettingDefinition);

							settingValue = parentSetting.ValueObject;

							settingValueSource = new SettingValueSource(SettingValueSourceType.Parent, resolutionContext.ParentEntity, parentSetting.ValueSource);
						}
					}
				}

				object setting = Activator.CreateInstance(
						settingDefinition.Declaration.PropertyType,
						new[] {settingValue, settingValueSource, null}
					);

				/*
				// ReSharper disable once PossibleNullReferenceException for obtaining Setting<> constructor, see PurposeDefinition<>.
				object setting = settingDefinition.Declaration.PropertyType.GetConstructor(
														BindingFlags.Public,
														null,
														new[]
														{
															settingDefinition.ValueType, _settingValueSourceType,
															_settingMetadataType
														},
														null
													)
												.Invoke(new[] {settingValue, settingValueSource, null});
												*/

				AssignSettingValue(purposeSettings, settingDefinition, setting);
			}

			return purposeSettings;
		}

		private ISettingsPurpose resolveSettings(EntityIdentifier entityIdentifier,
												string purpose,
												Type purposeType)
		{
			Func<EntityIdentifier, string, object> settingsResolver;
			
			lock(settingsResolvers)
			{
				if(!this.settingsResolvers.TryGetValue(purposeType,
														out settingsResolver))
				{
					Expression thisExpression = Expression.Constant(this);
					ParameterExpression parentEntityParameterExpression =
						Expression.Parameter(typeof(EntityIdentifier));
					ParameterExpression purposeParameterExpression = Expression.Parameter(typeof(string));

					Expression resolveSettingsExpression = Expression.Call(thisExpression, nameof(ResolveSettings),
																			new[] {purposeType},
																			parentEntityParameterExpression,
																			purposeParameterExpression);

					Expression<Func<EntityIdentifier, string, object>> settingsResolverExpression =
						Expression.Lambda<Func<EntityIdentifier, string, object>>(
								resolveSettingsExpression,
								parentEntityParameterExpression,
								purposeParameterExpression
							);

					settingsResolver = settingsResolverExpression.Compile();

					settingsResolvers.Add(purposeType, settingsResolver);
				}
			}

			object parentSettings = settingsResolver(entityIdentifier, purpose);
			
			return parentSettings as ISettingsPurpose;
		}

		private TPurpose ResolveSettings<TPurpose>(EntityIdentifier entity, string purpose, SettingResolutionContext resolutionContext)
			where TPurpose : class, ISettingsPurpose, new()
		{
			IEntityTypeSettingsSchema entityTypeSchema;
			IPurposeDefinition purposeDefinition;

			if(resolutionContext == null)
			{
				entityTypeSchema = this.Schema[entity.Type];
				purposeDefinition = entityTypeSchema[purpose];
			}
			else
			{
				entityTypeSchema = resolutionContext.EntityTypeSettingsSchema;
				purposeDefinition = resolutionContext.PurposeDefinition;
			}
			
			SettingResolutionContext settingResolutionContext = null;

			if(purposeDefinition.ParentSettingsInheritance == PurposeSettingsInheritanceType.Inherit &&
				!string.IsNullOrWhiteSpace(purposeDefinition.ParentEntityType))
			{
				IEntityTypeSettingsSchema parentEntityTypeSchema = Schema[purposeDefinition.ParentEntityType];

				IPurposeDefinition parentSchema = parentEntityTypeSchema[purpose];

				Type parentPurposeType = parentSchema.RunType;

				EntityIdentifier parentEntityIdentifier = new EntityIdentifier()
				{
					Type = purposeDefinition.ParentEntityType,
					Identifier =
						_entityRelationshipResolver.GetEntityParent(entity, purposeDefinition.ParentEntityType, purpose)
				};

				object parentSettings = resolveSettings(parentEntityIdentifier, purpose, parentPurposeType);

				settingResolutionContext = new SettingResolutionContext( entityTypeSchema, purposeDefinition, parentEntityTypeSchema, parentEntityIdentifier, parentSettings);
			}

			TPurpose settings = GetSettings<TPurpose>(entity, purpose, settingResolutionContext);
			return settings;
		}

		private void ValidateEntityIdentifier(EntityIdentifier entityIdentifier, out IEntityTypeSettingsSchema entityTypeSchema)
		{
			if(entityIdentifier == null) throw new ArgumentNullException(nameof(entityIdentifier));
			
			if(entityIdentifier.IsEmpty())
				throw new ArgumentException("Entity identifier is empty.", nameof(entityIdentifier));
			
			entityTypeSchema = Schema[entityIdentifier.Type];
			
			if( entityTypeSchema == null )
				throw new UnknownEntityType(entityIdentifier.Type);
		}

		private static void ValidatePurpose(string purpose, IEntityTypeSettingsSchema entityTypeSettingsSchema, out IPurposeDefinition purposeDefinition)
		{
			if(purpose == null) throw new ArgumentNullException(nameof(purpose));

			purposeDefinition = entityTypeSettingsSchema[purpose];
			
			if( purposeDefinition == null)
				throw new ArgumentOutOfRangeException(nameof(purpose), purpose,
													"Specified purpose does not exist within entity type.");
		}

		private static void ValidatePurpose<TPurpose>(string purpose, IEntityTypeSettingsSchema entityTypeSettingsSchema, out IPurposeDefinition purposeDefinition)
		{
			ValidatePurpose(purpose,  entityTypeSettingsSchema, out purposeDefinition);

			Type purposeType = typeof(TPurpose);

			if(purposeType != purposeDefinition.RunType)
				throw new PurposeException(
					purpose, $"Expected type '{purposeType.AssemblyQualifiedName}' for purpose '{purpose}' does not match run type '{purposeDefinition.RunType.AssemblyQualifiedName}' in entity type '{entityTypeSettingsSchema.Name}' schema.");
		}
		
		public TPurpose GetSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose)
			where TPurpose : class, ISettingsPurpose, new()
		{
			ValidateEntityIdentifier(entityIdentifier, out IEntityTypeSettingsSchema entityTypeSettingsSchema);
			ValidatePurpose<TPurpose>(purpose, entityTypeSettingsSchema, out IPurposeDefinition purposeDefinition);

			return GetSettings<TPurpose>(
					entityIdentifier, purpose, 
					new SettingResolutionContext( entityTypeSettingsSchema, purposeDefinition)
				);
		}

		public TPurpose GetEffectiveSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose)
			where TPurpose : class, ISettingsPurpose, new()
		{
			ValidateEntityIdentifier(entityIdentifier, out IEntityTypeSettingsSchema entityTypeSettingsSchema);
			ValidatePurpose<TPurpose>(purpose, entityTypeSettingsSchema, out IPurposeDefinition purposeDefinition);

			return ResolveSettings<TPurpose>(
					entityIdentifier, purpose, 
					new SettingResolutionContext( entityTypeSettingsSchema, purposeDefinition)
				);
		}

		public void SaveSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose, TPurpose settings)
			where TPurpose : class, ISettingsPurpose, new()
		{
			ValidateEntityIdentifier(entityIdentifier, out IEntityTypeSettingsSchema entityTypeSettingsSchema);
			ValidatePurpose<TPurpose>(purpose, entityTypeSettingsSchema, out IPurposeDefinition purposeDefinition);
			
			if(settings == null) throw new ArgumentNullException(nameof(settings));
			
			if( typeof(TPurpose) != purposeDefinition.RunType)
				throw new PurposeException(purpose, "Specified purpose type does not match schema");
		}

		public ISettingMetaData GetSettingMetaData(EntityIdentifier entityIdentifier, string purpose)
		{
			ValidateEntityIdentifier(entityIdentifier, out IEntityTypeSettingsSchema entityTypeSettingsSchema);
			ValidatePurpose(purpose, entityTypeSettingsSchema, out _);
			
			return null;
		}

		/*public ISettingMetaData GetSettingMetaData(EntityIdentifier identifier, string purpose, string setting)
		{
			return null;
		}*/
	}
}