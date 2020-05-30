using System;

namespace Settings.Schema
{
	public class NotInheritable : Exception
	{
		public NotInheritable(string message) : base(message)
		{
		}
	}
}