using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Settings.Schema
{
	internal class EntityTypePurpose
	{
		private readonly IDictionary<string, SettingDefinition> settingDefinitions;

		public EntityTypePurpose(string name, IDictionary<string, SettingDefinition> settingDefinitions)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			
			Name = name;
			this.settingDefinitions = settingDefinitions;
		}

		public string Name { get; }

		internal IDictionary<string, SettingDefinition> Settings =>
			new ReadOnlyDictionary<string, SettingDefinition>(settingDefinitions);

		public IEnumerable<SettingDefinition> GetSettings()
		{
			return settingDefinitions.Values;
		}

		public SettingDefinition GetSetting(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

			if(settingDefinitions.ContainsKey(name))
				return settingDefinitions[name];

			return null;
		}
	}
}