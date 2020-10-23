using System;

namespace Settings.Schema
{
	public class UnknownPurpose : Exception
	{
		public UnknownPurpose() : this(String.Empty)
		{
		}

		public UnknownPurpose(string name) : base()
		{
			Name = name;
		}

		public UnknownPurpose(string name, string message) : base(message)
		{
			this.Name = name;
		}

		public string Name { get; }
	}
}