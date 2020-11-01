using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Settings.Schema;

namespace Tensible
{
	internal class Unified : IUnified
	{
		private IDictionary<string, IDictionary<string, ISettingsResolver>> entityTypes = null;
		private readonly IServiceProvider _serviceProvider;

		private static void ValidateSettingsCriteria<TEntity>(string entityType, string purpose, TEntity entity)
		{
			if(entityType == null) throw new ArgumentNullException(nameof(entityType));
			if(purpose == null) throw new ArgumentNullException(nameof(purpose));
			if(entity == null) throw new ArgumentNullException(nameof(entity));
		}

		internal Unified(IDictionary<string, IDictionary<string, ISettingsResolver>> entityTypes, IServiceCollection serviceCollection)
		{
			this.entityTypes = entityTypes;

			serviceCollection.AddSingleton<IUnified>(this);
			_serviceProvider = serviceCollection.BuildServiceProvider();
		}
		
		ISettingsResolver GetSettings(string entityType, string purpose)
		{
			if(!entityTypes.TryGetValue(entityType, out IDictionary<string, ISettingsResolver> entitySettings))
				throw new UnknownEntityType();
			
			if( !entitySettings.TryGetValue(purpose, out ISettingsResolver settings))
				throw new UnknownPurpose();

			return settings;
		}

		public TPurpose GetEffectiveSettings<TEntity, TPurpose>(string entityType, string purpose, TEntity entity) where TPurpose : class
		{
			ValidateSettingsCriteria(entityType, purpose, entity);

			return GetSettings(entityType, purpose).GetEffective<TEntity, TPurpose>(entity, _serviceProvider) as TPurpose;
		}
		
		public TPurpose GetSettings<TEntity, TPurpose>(string entityType, string purpose, TEntity entity) where TPurpose : class
		{
			ValidateSettingsCriteria(entityType, purpose, entity);
			
			return GetSettings(entityType, purpose).Get<TEntity, TPurpose>(entity, _serviceProvider) as TPurpose;
		}

		public TPurpose GetEffectiveSettings<TEntity, TPurpose>(TEntity entity) where TPurpose : class
		{
			return GetEffectiveSettings<TEntity, TPurpose>(typeof(TPurpose).FullName, typeof(TPurpose).FullName, entity);
		}

		public TPurpose GetEffectiveSettings<TEntity, TPurpose>(string purpose, TEntity entity) where TPurpose : class
		{
			return GetEffectiveSettings<TEntity, TPurpose>(typeof(TPurpose).FullName, purpose, entity);
		}
		
		public TPurpose GetSettings<TEntity, TPurpose>(TEntity entity) where TPurpose : class
		{
			return GetSettings<TEntity, TPurpose>(typeof(TPurpose).FullName, typeof(TPurpose).FullName, entity);
		}

		public TPurpose GetSettings<TEntity, TPurpose>(string purpose, TEntity entity) where TPurpose : class
		{
			return GetSettings<TEntity, TPurpose>(typeof(TPurpose).FullName, purpose, entity);
		}
	}
}