using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Tensible
{
	internal class Entity<TEntity> : IEntity<TEntity>, IEntityDefinition
		where TEntity : class
	{
		private Dictionary<string, ISettingsDefinition> settings = new Dictionary<string, ISettingsDefinition>();

		public Entity(string name)
		{
			this.Name = name;
		}

		public string Name { get; }

		public IEntity<TEntity> With<TPurpose>(ISettings<TEntity, TPurpose> settings) where TPurpose : class
		{
			if(settings == null) throw new ArgumentNullException(nameof(settings));

			lock(settings)
			{
				if(this.settings.ContainsKey(settings.Name))
					throw new DuplicateNameException($"Settings of name '{settings.Name}' already exists.");

				this.settings.Add(settings.Name, settings);
			}

			return this;
		}

		public IDictionary<string, ISettingsDefinition> Settings => new ReadOnlyDictionary<string, ISettingsDefinition>( this.settings);
	}
}