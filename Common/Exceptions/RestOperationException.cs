namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class RestOperationException : BaseLoggingException
    {
        public RestOperationException()
        {
        }

        public RestOperationException(string message) : base(message)
        {
        }

        public RestOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RestOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}