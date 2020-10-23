using Pug.Effable;

namespace Pug.Settings
{
	public interface ISettingMetaData : 
		IRegistered<string, IRegistrationInfo<string>>, 
		IUpdatable<string, IUpdateInfo<string>>
	{
		
	}
}