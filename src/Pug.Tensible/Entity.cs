using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Pug.Tensible
{
	internal class Entity<TEntity> : IEntity<TEntity>, IEntityDefinition
		where TEntity : class
	{
		private Dictionary<string, ISettingsDefinition> _settings = new Dictionary<string, ISettingsDefinition>();

		public Entity(string name)
		{
			Name = name;
		}

		public string Name { get; }

		private void Register<TSettings>(ISettings<TEntity, TSettings> settings) where TSettings : class
		{
			lock(settings)
			{
				if(_settings.ContainsKey(settings.Name))
					throw new DuplicateNameException($"Settings of '{settings.Name}' purpose already exists.");

				_settings.Add(settings.Name, settings);
			}
		}

		public IEntity<TEntity> With<TSettings>(ISettings<TEntity, TSettings> settings) where TSettings : class
		{
			if(settings == null) throw new ArgumentNullException(nameof(settings));

			Register(settings);

			return this;
		}

		public IEntity<TEntity> With<TSettings>(Func<TEntity, IServiceProvider, TSettings> settingsAccessor,
												Action<ISettings<TEntity, TSettings>> configurator)
			where TSettings : class
		{
			if(settingsAccessor == null) throw new ArgumentNullException(nameof(settingsAccessor));
			
			Settings<TEntity, TSettings> settings = new Settings<TEntity, TSettings>(typeof(TSettings).FullName, settingsAccessor);

			configurator?.Invoke(settings);

			Register(settings);
			
			return this;
		}

		public IEntity<TEntity> With<TSettings>(string name,
												Func<TEntity, IServiceProvider, TSettings> settingsAccessor,
												Action<ISettings<TEntity, TSettings>> configurator)
			where TSettings : class
		{
			if(settingsAccessor == null) throw new ArgumentNullException(nameof(settingsAccessor));
			
			Settings<TEntity, TSettings> settings = new Settings<TEntity, TSettings>(name, settingsAccessor);

			configurator?.Invoke(settings);

			Register(settings);
			
			return this;
		}


		public IDictionary<string, ISettingsDefinition> Settings => new ReadOnlyDictionary<string, ISettingsDefinition>( _settings);
	}
}