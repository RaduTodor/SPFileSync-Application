namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class NoInternetAccessException : BaseLoggingException
    {
        public NoInternetAccessException()
        {
        }

        public NoInternetAccessException(string message) : base(message)
        {
        }

        public NoInternetAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoInternetAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}