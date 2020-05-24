using System;

namespace Settings.Schema
{
	public class UnknownPurpose : Exception
	{
		public UnknownPurpose()
		{
		}

		public UnknownPurpose(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}