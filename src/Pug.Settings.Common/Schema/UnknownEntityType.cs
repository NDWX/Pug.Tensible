using System;

namespace Settings.Schema
{
	public class UnknownEntityType : Exception
	{
		public string EntityType { get; }

		public UnknownEntityType() : base()
		{
			
		}

		public UnknownEntityType(string entityType) : base()
		{
			EntityType = entityType;
		}
}
}