using System;
using System.Runtime.Serialization;

namespace Demo.Retailer.Api.Models
{
	[Serializable]
	internal class PageOutOfRangeException : Exception
	{
		public PageOutOfRangeException() : base("Requested page is out of range")
		{
		}

		public PageOutOfRangeException(string message) : base(message)
		{
		}

		public PageOutOfRangeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected PageOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}