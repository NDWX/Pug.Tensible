using Pug.Settings.Schema;

namespace Pug.Settings
{
	internal class ReadOnlySchema : ISettingsSchema
	{
		private readonly ISettingsSchema _schema;

		public ReadOnlySchema(ISettingsSchema schema)
		{
			_schema = schema;
		}
		
		public IEntityTypeSettingsSchema AddEntityType(string name)
		{
			throw new SettingsSchemaException("Schema can no longer be modified.");
		}

		public IEntityTypeSettingsSchema this[string name]
		{
			get
			{
				IEntityTypeSettingsSchema entityTypeSettingsSchema = _schema[name];

				if(entityTypeSettingsSchema == null)
					return null;
				
				return new ReadOnlyEntityTypeSettingsSchema(entityTypeSettingsSchema);
			}
		}
	}
}