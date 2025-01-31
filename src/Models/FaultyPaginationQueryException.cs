using System;
using System.Runtime.Serialization;

namespace API.Models
{
	[Serializable]
	public class FaultyPaginationQueryException : Exception
	{
		public FaultyPaginationQueryException()
		{
		}

		public FaultyPaginationQueryException(string message) : base(message)
		{
		}

		public FaultyPaginationQueryException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FaultyPaginationQueryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}