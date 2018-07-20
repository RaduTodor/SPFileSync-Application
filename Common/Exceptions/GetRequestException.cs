namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class GetRequestException : BaseLoggingException
    {
        public GetRequestException()
        {
        }

        public GetRequestException(string message) : base(message)
        {
        }

        public GetRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GetRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
