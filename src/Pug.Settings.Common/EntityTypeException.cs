using System;

namespace Pug.Settings
{
	public class EntityTypeException : Exception
	{
		public string Name { get; }

		public EntityTypeException(string name, string message) : base(message)
		{
			Name = name;
		}
	}
}