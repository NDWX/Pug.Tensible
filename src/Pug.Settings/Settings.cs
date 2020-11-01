using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tensible
{
	public class Settings<TEntity, TPurpose> : ISettings<TEntity, TPurpose> 
		where TEntity : class
		where TPurpose : class
	{
		/*private readonly LinkedList<Func<string, TPurpose, TPurpose>> parentSettingsMapper =
			new LinkedList<Func<string, TPurpose, TPurpose>>();*/

		private readonly List<SettingsParent<TEntity, TPurpose>> parentSettings = new List<SettingsParent<TEntity, TPurpose>>();

		public string Name { get; }

		public Func<TEntity, IServiceProvider, TPurpose> SettingsAccessor { get; }

		public Settings(string name, Func<TEntity, IServiceProvider, TPurpose> settingsAccessor)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			SettingsAccessor = settingsAccessor ?? throw new ArgumentNullException(nameof(settingsAccessor));
		}

		public Settings(Func<TEntity, IServiceProvider, TPurpose> settingsAccessor) : this(typeof(TPurpose).FullName, settingsAccessor)
		{
		}

		private void AddSettingsParent<TParentEntity, TParentSettings>(string parentEntityType,
																		string parentSettingsPurposeName,
																		Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper, Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
																		Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap)
		{
			Func<TEntity, TPurpose, IServiceProvider, TPurpose> lambda = (entity, baseSettings, serviceProvider) =>
			{
				TParentEntity parentEntity = entityParentMapper(entity, serviceProvider);

				if(parentEntity != null)
				{
					TParentSettings parentSettings = settingsAccessor(parentEntity, serviceProvider);

					if(parentSettings != null) return settingsMap(baseSettings, parentSettings, serviceProvider);
				}

				return baseSettings;
			};

			parentSettings.Add(
					new SettingsParent<TEntity, TPurpose>(parentEntityType, parentSettingsPurposeName, lambda)
				);
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap)
		{
			if(parentSettingsPurposeName == null) throw new ArgumentNullException(nameof(parentSettingsPurposeName));
			if(settingsAccessor == null) throw new ArgumentNullException(nameof(settingsAccessor));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			if(string.IsNullOrWhiteSpace(parentEntityType))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(parentEntityType));
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			/*
			ParameterExpression entityParameterExpression =
				Expression.Parameter(typeof(TEntity), "entity");
			ParameterExpression entitySettingsParameterExpression =
				Expression.Parameter(typeof(TPurpose), "baseSettings");
			ParameterExpression serviceProviderParameterExpression =
				Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

			ParameterExpression parentEntityVariableExpression = Expression.Variable(typeof(TParentEntity), "parentEntity");
			LabelTarget returnTargetExpression = Expression.Label(typeof(TPurpose));
			ParameterExpression parentSettingsVariableExpression =
				Expression.Variable(typeof(TParentSettings), "parentSettings");
			
			parentSettings.Add(
					new SettingsParent<TEntity, TPurpose>(
							parentEntityType, parentSettingsPurposeName,
							// Func<string, TPurpose, TPurpose> lambda = (entity, baseSettings) =>
							Expression.Lambda<Func<TEntity, TPurpose, IServiceProvider, TPurpose>>(
									// {
									Expression.Block(
											// TParentEntity parentEntity = entityParentMapper(entity, serviceProvider);
											Expression.Assign(
													parentEntityVariableExpression,
													Expression.Call(entityParentMapper.Method, entityParameterExpression, serviceProviderParameterExpression)
													),
											// TParentPurpose parentSettings;
											parentSettingsVariableExpression,
											// parentSettings = settingsAccessor(parentEntity, serviceProvider);
											Expression.Assign(
													parentSettingsVariableExpression,
													Expression.Call(settingsAccessor.Method,
																	parentEntityVariableExpression,
																	serviceProviderParameterExpression)
												),
											// if(parentSettings != null) return settingsMap(purpose, parentSettings);
											Expression.IfThen(
													// (parentSettings != null)
													Expression.NotEqual(
														parentSettingsVariableExpression,
														Expression.Constant(null)),
													//return settingsMap(purpose, parentSettings, serviceProvider);
													Expression.Return(
															returnTargetExpression,
															Expression.Call(
																settingsMap.Method,
																entitySettingsParameterExpression,
																parentSettingsVariableExpression,
																serviceProviderParameterExpression)
														)
												),
											// return purpose;
											Expression.Return(
												returnTargetExpression,
												entitySettingsParameterExpression)
											// }
										), // Expression.Block
									entityParameterExpression,
									entitySettingsParameterExpression,
									serviceProviderParameterExpression
								).Compile()
						) // new SettingsParent<TPurpose>
				); // parentSettingsMapper.AddLast
*/

			return this;
		}

		private static Func<TParentEntity, IServiceProvider, TParentSettings> GetParentSettingAccessor<TParentEntity, TParentSettings>(
			string parentEntityType, string parentSettingsPurposeName, bool useEffectiveSettings = true) where TParentSettings : class
		{
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor;
			if(useEffectiveSettings)
				settingsAccessor = (entity, serviceProvider) =>
					((IUnified) serviceProvider.GetService(typeof(IUnified)))
					.GetEffectiveSettings<TParentEntity, TParentSettings>(
						parentEntityType, parentSettingsPurposeName, entity);
			else
				settingsAccessor = (entity, serviceProvider) =>
					((IUnified) serviceProvider.GetService(typeof(IUnified)))
					.GetSettings<TParentEntity, TParentSettings>(
						parentEntityType, parentSettingsPurposeName, entity);
			
			return settingsAccessor;
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class
		{
			if(parentEntityType == null) throw new ArgumentNullException(nameof(parentEntityType));
			if(parentSettingsPurposeName == null) throw new ArgumentNullException(nameof(parentSettingsPurposeName));
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName,
					useEffectiveSettings);
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class
		{
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName);
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		/*
		public SettingsDefinition<TSettings> BasedOn<TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		)
		{
			return BasedOn(
					parentEntityType,
					parentSettingsPurposeName,
					(entityIdentifier, serviceProvider) => (serviceProvider.GetService(typeof(IUnified)) as IUnified).GetEffectiveSettings<TParentSettings>(parentEntityType, parentSettingsPurposeName, )
				);
		}
		
		*/
		
		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		)
		{
			return BasedOn<TParentEntity, TParentSettings>(parentEntityType,  typeof(TParentSettings).FullName, entityParentMapper, settingsAccessor, settingsMap);
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		)
		{
			return BasedOn(typeof(TParentEntity).FullName, typeof(TParentSettings).FullName, entityParentMapper, settingsAccessor, settingsMap);
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class
		{
			if(parentEntityType == null) throw new ArgumentNullException(nameof(parentEntityType));
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			
			string parentSettingsPurposeName = typeof(TParentSettings).FullName;
			
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName);
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class
		{
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			
			string parentEntityType = typeof(TParentEntity).FullName;
			string parentSettingsPurposeName = typeof(TParentSettings).FullName;
			
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName);
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class
		{
			if(parentEntityType == null) throw new ArgumentNullException(nameof(parentEntityType));
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			
			string parentSettingsPurposeName = typeof(TParentSettings).FullName;
			
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName,
					useEffectiveSettings);
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TPurpose> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TPurpose, TParentSettings, IServiceProvider, TPurpose> settingsMap
		) where TParentSettings : class
		{
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			
			string parentEntityType = typeof(TParentEntity).FullName;
			string parentSettingsPurposeName = typeof(TParentSettings).FullName;
			
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName,
					useEffectiveSettings);
			
			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		ISettingsResolver ISettingsDefinition.GetResolver()
		{
			return new SettingsResolver<TEntity, TPurpose>(this, SettingsAccessor, parentSettings);
		}
	}
}