namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class BaseLoggingException : Exception
    {
        public BaseLoggingException()
        {
        }

        public BaseLoggingException(string message) : base(message)
        {
        }

        public BaseLoggingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BaseLoggingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}