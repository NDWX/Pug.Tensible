using System;
using System.Runtime.Serialization;

namespace Settings.Schema
{
	[Serializable]
	public class UnknownEntityTypeException : Exception
	{
		private const string EntityTypeFieldName = "EntityType";
		public string EntityType { get; }

		public UnknownEntityTypeException() : base()
		{
			
		}

		public UnknownEntityTypeException(string entityType) : base()
		{
			EntityType = entityType;
		}
		

		protected UnknownEntityTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			EntityType = info.GetString(EntityTypeFieldName);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(EntityTypeFieldName, EntityType);
		}
	}
}