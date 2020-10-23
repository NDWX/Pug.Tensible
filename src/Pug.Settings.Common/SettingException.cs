using System;

namespace Pug.Settings
{
	public class SettingException : Exception
	{
		public string Name { get; }

		public SettingException(string name, string message) : base(message)
		{
			Name = name;
		}
	}
}