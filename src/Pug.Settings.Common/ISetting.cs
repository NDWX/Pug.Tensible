using Settings;

namespace Pug.Settings
{
	public interface ISetting
	{
		SettingValueSource ValueSource { get; }
		
		object ValueObject { get; }
	}
}