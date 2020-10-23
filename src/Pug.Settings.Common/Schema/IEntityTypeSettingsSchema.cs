namespace Pug.Settings.Schema
{
	public interface IEntityTypeSettingsSchema
	{
		string Name { get; }
		
		IEntityTypeSettingsSchema With<TPurpose>(Purpose<TPurpose> purpose) where TPurpose : class, ISettingsPurpose, new();
		
		IPurposeDefinition this[string purpose] { get; }
	}
}