namespace Pug.Settings
{
	public interface ISettingValueSerializationProvider
	{
		TValue Deserialize<TValue>(string valueString);

		string Serialize<TValue>(TValue value);
	}
}