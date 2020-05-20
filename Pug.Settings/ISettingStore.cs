namespace Settings
{
	public interface ISettingStore
	{
		Setting GetSetting(EntityIdentifier entity, string purpose, string name);
	}
}