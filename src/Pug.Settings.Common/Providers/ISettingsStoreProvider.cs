using System.Collections.Generic;
using Settings;

namespace Pug.Settings
{
	public interface ISettingsStoreProvider
	{
		IDictionary<string, string> GetSettings(EntityIdentifier entity, string purpose);

		ISettingMetaData GetPurposeMetaData(EntityIdentifier entity, string purpose);

		void Store(EntityIdentifier entity, string purpose, IDictionary<string, string> settings, string user);
	}
}