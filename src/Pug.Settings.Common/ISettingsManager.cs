using Pug.Settings.Schema;
using Settings;

namespace Pug.Settings
{
	public interface ISettingsManager
	{
		ISettingsSchema Schema { get; }

		TPurpose GetSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose)
			where TPurpose : class, ISettingsPurpose, new();

		TPurpose GetEffectiveSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose)
			where TPurpose : class, ISettingsPurpose, new();

		void SaveSettings<TPurpose>(EntityIdentifier entityIdentifier, string purpose, TPurpose settings)
			where TPurpose : class, ISettingsPurpose, new();
		
		ISettingMetaData GetSettingMetaData(EntityIdentifier entityIdentifier, string purpose);
	}
}