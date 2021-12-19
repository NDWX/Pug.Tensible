using System;
using System.Runtime.Serialization;

namespace Pug.Tensible
{
	[Serializable]
	public class SettingsException : Exception
	{
		public SettingsException(string message) : base(message)
		{

		}

		public SettingsException(string message, Exception innerException) : base(message, innerException)
		{

		}

		protected SettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}
}