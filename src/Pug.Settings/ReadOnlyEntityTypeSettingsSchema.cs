using Pug.Settings.Schema;

namespace Pug.Settings
{
	internal class ReadOnlyEntityTypeSettingsSchema : IEntityTypeSettingsSchema
	{
		private readonly IEntityTypeSettingsSchema _schema;

		public ReadOnlyEntityTypeSettingsSchema(IEntityTypeSettingsSchema schema)
		{
			_schema = schema;
		}

		public string Name => _schema.Name;
		
		public IEntityTypeSettingsSchema With<TPurpose>(Purpose<TPurpose> purpose) 
			where TPurpose :  class, ISettingsPurpose, new()
		{
			throw new SettingsSchemaException("Schema can no longer be modified.");
		}

		public IPurposeDefinition this[string purpose] => _schema[purpose];
	}
}