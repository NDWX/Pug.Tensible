using System.Collections.Generic;

namespace Settings
{
	public interface ISettingStore
	{
		Setting GetSetting(EntityIdentifier entity, string purpose, string name);
		
		IEnumerable<Setting> GetSettings(EntityIdentifier entity, string purpose);
	}
}