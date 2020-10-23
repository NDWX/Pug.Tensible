namespace Pug.Settings.Schema
{
	public interface ISettingsSchema
	{
		IEntityTypeSettingsSchema AddEntityType(string name);
		
		IEntityTypeSettingsSchema this[string name] { get; }
	}
}