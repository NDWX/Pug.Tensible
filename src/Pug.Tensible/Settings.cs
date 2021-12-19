using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Pug.Tensible
{
	public class Settings<TEntity, TSettings> : ISettings<TEntity, TSettings>
		where TEntity : class
		where TSettings : class
	{
		private readonly List<string> parentEntities = new List<string>();
		private readonly List<SettingsParent<TEntity, TSettings>> _parentSettings =
			new List<SettingsParent<TEntity, TSettings>>();

		public IEnumerable<string> ParentEntities => parentEntities;
		
		public string Name { get; }

		public Func<TEntity, IServiceProvider, TSettings> SettingsAccessor { get; }

		public Settings(string name, Func<TEntity, IServiceProvider, TSettings> settingsAccessor)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			SettingsAccessor = settingsAccessor ?? throw new ArgumentNullException(nameof(settingsAccessor));
		}

		public Settings(Func<TEntity, IServiceProvider, TSettings> settingsAccessor)
			: this(typeof(TSettings).FullName, settingsAccessor)
		{
		}

		#region Stepped parent settings mapping
		
		private void AddSettingsParent<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMap,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap)
		{
			Func<TEntity, TSettings, IServiceProvider, TSettings> lambda = (entity, baseSettings, serviceProvider) =>
			{
				var parentEntity = entityParentMap(entity, serviceProvider);

				if(parentEntity != null)
				{
					TParentSettings parentSettings = parentSettingsAccessor(parentEntity, serviceProvider);

					if(parentSettings != null) return settingsMap(baseSettings, parentSettings, serviceProvider);
				}

				return baseSettings;
			};

			_parentSettings.Add(
					new SettingsParent<TEntity, TSettings>(parentEntityType, parentSettingsPurposeName, lambda)
				);
				
			if( !parentEntities.Contains(parentEntityType))
				parentEntities.Add(parentEntityType);
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap)
		{
			if(parentSettingsPurposeName == null) throw new ArgumentNullException(nameof(parentSettingsPurposeName));
			if(parentSettingsAccessor == null) throw new ArgumentNullException(nameof(parentSettingsAccessor));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			if(string.IsNullOrWhiteSpace(parentEntityType))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(parentEntityType));

			AddSettingsParent(
				parentEntityType, parentSettingsPurposeName, entityParentMapper, parentSettingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		)
		{
			return BasedOn<TParentEntity, TParentSettings>(parentEntityType, typeof(TParentSettings).FullName,
															entityParentMapper, parentSettingsAccessor, settingsMap);
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TParentEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		)
		{
			return BasedOn(typeof(TParentEntity).FullName, typeof(TParentSettings).FullName, entityParentMapper,
							parentSettingsAccessor, settingsMap);
		}
		
		#endregion

		#region Custom parent settings mapping
		
		private void AddSettingsParent<TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap)
		{
			Func<TEntity, TSettings, IServiceProvider, TSettings> lambda = (entity, baseSettings, serviceProvider) =>
			{
				TParentSettings parentSettings = parentSettingsAccessor(entity, serviceProvider);

				if(parentSettings != null) return settingsMap(baseSettings, parentSettings, serviceProvider);

				return baseSettings;
			};

			_parentSettings.Add(
					new SettingsParent<TEntity, TSettings>(parentEntityType, parentSettingsPurposeName, lambda)
				);
				
			if( !parentEntities.Contains(parentEntityType))
				parentEntities.Add(parentEntityType);
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap)
		{
			if(parentSettingsPurposeName == null) throw new ArgumentNullException(nameof(parentSettingsPurposeName));
			if(parentSettingsAccessor == null) throw new ArgumentNullException(nameof(parentSettingsAccessor));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));
			
			if(string.IsNullOrWhiteSpace(parentEntityType))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(parentEntityType));

			AddSettingsParent(
				parentEntityType, parentSettingsPurposeName, parentSettingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		)
		{
			return BasedOn(parentEntityType, typeof(TParentSettings).FullName, 
															parentSettingsAccessor, settingsMap);
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentSettings> parentSettingsAccessor,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		)
		{
			return BasedOn(typeof(TParentEntity).FullName, typeof(TParentSettings).FullName,
							parentSettingsAccessor, settingsMap);
		}

		#endregion
		
		#region Auto recurse parent settings mapping
		
		private static Func<TParentEntity, IServiceProvider, TParentSettings> GetDefaultParentSettingAccessor<
			TParentEntity, TParentSettings>(
			string parentEntityType, string parentSettingsPurposeName, bool useEffectiveSettings = true)
			where TParentSettings : class
		{
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor;

			if(useEffectiveSettings)
				settingsAccessor = (entity, serviceProvider) =>
					((IUnified) serviceProvider.GetService(typeof(IUnified)))
						.GetEffectiveSettings<TParentEntity, TParentSettings>(
																				parentEntityType,
																				parentSettingsPurposeName, 
																				entity);
			else
				settingsAccessor = (entity, serviceProvider) =>
					((IUnified) serviceProvider.GetService(typeof(IUnified)))
						.GetSettings<TParentEntity, TParentSettings>(
																		parentEntityType, 
																		parentSettingsPurposeName, 
																		entity);

			return settingsAccessor;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class
		{
			if(parentEntityType == null) throw new ArgumentNullException(nameof(parentEntityType));
			if(parentSettingsPurposeName == null) throw new ArgumentNullException(nameof(parentSettingsPurposeName));
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));

			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetDefaultParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName,
					useEffectiveSettings);

			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			string parentSettingsPurposeName,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class
		{
			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetDefaultParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName);

			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class
		{
			if(parentEntityType == null) throw new ArgumentNullException(nameof(parentEntityType));
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));

			string parentSettingsPurposeName = typeof(TParentSettings).FullName;

			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetDefaultParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName);

			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class
		{
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));

			string parentEntityType = typeof(TParentEntity).FullName;
			string parentSettingsPurposeName = typeof(TParentSettings).FullName;

			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetDefaultParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName);

			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			string parentEntityType,
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class
		{
			if(parentEntityType == null) throw new ArgumentNullException(nameof(parentEntityType));
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));

			string parentSettingsPurposeName = typeof(TParentSettings).FullName;

			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetDefaultParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName,
					useEffectiveSettings);

			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		public ISettings<TEntity, TSettings> BasedOn<TParentEntity, TParentSettings>(
			Func<TEntity, IServiceProvider, TParentEntity> entityParentMapper,
			bool useEffectiveSettings,
			Func<TSettings, TParentSettings, IServiceProvider, TSettings> settingsMap
		) where TParentSettings : class
		{
			if(entityParentMapper == null) throw new ArgumentNullException(nameof(entityParentMapper));
			if(settingsMap == null) throw new ArgumentNullException(nameof(settingsMap));

			string parentEntityType = typeof(TParentEntity).FullName;
			string parentSettingsPurposeName = typeof(TParentSettings).FullName;

			Func<TParentEntity, IServiceProvider, TParentSettings> settingsAccessor =
				GetDefaultParentSettingAccessor<TParentEntity, TParentSettings>(
					parentEntityType,
					parentSettingsPurposeName,
					useEffectiveSettings);

			AddSettingsParent(parentEntityType, parentSettingsPurposeName, entityParentMapper, settingsAccessor, settingsMap);

			return this;
		}

		#endregion
		
		ISettingsResolver ISettingsDefinition.GetResolver(IEntityDefinition entityDefinition)
		{
			return new SettingsResolver<TEntity, TSettings>(this, SettingsAccessor, _parentSettings);
		}
	}
}