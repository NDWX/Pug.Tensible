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
									ValueSource = new SettingValueSource(SettingValueSourceType.User, entity),
									Value = "Test Value"
								};
							else if(purpose == "Purpose1" && name == "Setting2")
								setting = new Setting
								{
									Name = name,
									Purpose = purpose,
									ValueSource = new SettingValueSource(SettingValueSourceType.Default, entity),
									Value = "Old Default Value"
								};
							else if(purpose == "Purpose1" && name == "Setting3")
								setting = new Setting
								{
									Name = name,
									Purpose = purpose,
									ValueSource = new SettingValueSource(SettingValueSourceType.User, entity),
									Value = "E1P1S3Value"
								};

							break;

						default:
							return null;
					}

					break;

				case "SecondType":
					// switch(entity.Identifier)
					// {
					// 	case "SecondEntity":
					//
					// 		if(purpose == "Purpose1" && name == "Setting1")
					// 			setting = new Setting
					// 			{
					// 				Name = name,
					// 				Purpose = purpose,
					// 				ValueSource = new SettingValueSource(SettingValueSourceType.User, entity.Type),
					// 				Value = "E2P1S1Value"
					// 			};
					//
					// 		break;
					//
					// 	default:
					// 		return null;
					// }
					break;
			}

			return setting;
		}

		public IEnumerable<Setting> GetSettings(EntityIdentifier entity, string purpose)
		{
			return new[]
			{
				new Setting
				{
					Name = "Setting1",
					Purpose = purpose,
					ValueSource = new SettingValueSource( SettingValueSourceType.User, entity),
					Value = "Test Value"
				},
				new Setting
				{
					Name = "Setting2",
					Purpose = purpose,
					ValueSource = new SettingValueSource(SettingValueSourceType.Default, entity),
					Value = "Old Default Value"
				}
			};
		}

		#endregion
	}
}