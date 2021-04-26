using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tensible
{
	public class Unifier : IUnifier
	{
		private readonly IServiceCollection _serviceCollection;
		private readonly Dictionary<string, IEntityDefinition> entities = new Dictionary<string, IEntityDefinition>();

		public Unifier(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
		}

		public Unifier(Action<IServiceCollection> dependencyInjector) 
			: this(new ServiceCollection())
		{
			if( dependencyInjector != null )
				dependencyInjector(_serviceCollection);
		}
		
		public IEntity<TEntity> AddEntity<TEntity>() where TEntity : class
		{
			return AddEntity<TEntity>(typeof(TEntity).FullName);
		}

		public IEntity<TEntity> AddEntity<TEntity>(string name) where TEntity : class
		{
			if(name == null) throw new ArgumentNullException(nameof(name));

			Entity<TEntity> entity;
			
			lock(entities)
			{
				if(entities.ContainsKey(name))
					throw new DuplicateNameException($"Settings of name '{name}' already exists.");

				entity = new Entity<TEntity>(name);

				entities.Add(name, entity);
			}
			
			return entity;
		}

		public IUnified Unify()
		{
			Dictionary<string, IDictionary<string, ISettingsResolver>> entityTypes = new Dictionary<string, IDictionary<string, ISettingsResolver>>(entities.Count);
			
			foreach(var entity in entities.Values)
			{
				entityTypes.Add(entity.Name,
								entity.Settings.ToDictionary(x => x.Key,
															x => x.Value.GetResolver(entity))
					);
			}

			return new Unified(entityTypes, _serviceCollection);
		}
	}
}