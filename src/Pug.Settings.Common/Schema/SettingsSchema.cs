using System;
using System.Collections.Generic;

namespace Pug.Settings.Schema
{
	public class SettingsSchema : ISettingsSchema
	{
		private Dictionary<string, EntityTypeSettingsSchema> entityTypes = new Dictionary<string, EntityTypeSettingsSchema>();
		
		public IEntityTypeSettingsSchema AddEntityType(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
			
			if( entityTypes.ContainsKey(name) )
				throw new EntityTypeException(name, $"Entity type '{name}' is already used.");
			
			EntityTypeSettingsSchema settingsSchema = new EntityTypeSettingsSchema(name, this);
			
			entityTypes.Add(name, settingsSchema);

			return settingsSchema;
		}

		public IEntityTypeSettingsSchema this[string name]
		{
			get
			{
				if(!entityTypes.ContainsKey(name))
					return null;

				return entityTypes[name];
			}
		}
	}
}