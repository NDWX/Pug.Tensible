using System;

namespace Settings.Schema
{
	public class UnknownSetting : Exception
	{
		public string Name { get; }

		public UnknownSetting() : base()
		{
		}

		public UnknownSetting(string name) : base()
		{
			Name = name;
		}
	}
}