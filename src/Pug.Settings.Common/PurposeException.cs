using System;

namespace Pug.Settings
{
	public class PurposeException : Exception
	{
		public string Name { get; }

		public PurposeException(string name, string message) : base(message)
		{
			Name = name;
		}
	}
}