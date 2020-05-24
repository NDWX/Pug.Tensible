using System.Collections.Generic;
using Settings;

namespace UnitTests
{
	public class DummySettingStore : ISettingStore
	{
		#region ISettingStore Members

		public Setting GetSetting(EntityIdentifier entity, string purpose, string name)
		{
			Setting setting = null;

			switch(entity.Type)
			{
				case "FirstType":
					switch(entity.Identifier)
					{
						case "FirstEntity":

							if(purpose == "Purpose1" && name == "Setting1")
								setting = new Setting
								{
									Name = name,
									Purpose = purpose,
									ValueSource = new SettingValueSource
									{
										Type = SettingValueSourceType.User
									},
									Value = "Test Value"
								};
							else if(purpose == "Purpose1" && name == "Setting2")
								setting = new Setting
								{
									Name = name,
									Purpose = purpose,
									ValueSource = new SettingValueSource
									{
										Type = SettingValueSourceType.Default
									},
									Value = "Old Default Value"
								};

							break;

						default:
							return null;
					}

					break;

				case "SecondType":
					break;
			}

			return setting;
		}

		public IEnumerable<Setting> GetSettings(EntityIdentifier entity, string purpose)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}