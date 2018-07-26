namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class BaseLoggingException : Exception
    {
        //TODO [CR RT] Please make ctors protected where possible
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