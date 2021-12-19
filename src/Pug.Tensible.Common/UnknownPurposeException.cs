using System;
using System.Runtime.Serialization;

namespace Settings.Schema
{
	[Serializable]
	public class UnknownPurposeException : Exception
	{
		private const string PurposeNameFieldName = "PurposeName";

		public UnknownPurposeException() : this(String.Empty)
		{
		}

		public UnknownPurposeException(string name) : base()
		{
			Name = name;
		}

		public UnknownPurposeException(string name, string message) : base(message)
		{
			this.Name = name;
		}

		public string Name { get; }

		protected UnknownPurposeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Name = info.GetString(PurposeNameFieldName);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PurposeNameFieldName, Name);
		}
	}
}