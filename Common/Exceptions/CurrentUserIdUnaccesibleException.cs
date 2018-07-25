namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class CurrentUserIdUnaccesibleException : BaseLoggingException
    {
        public CurrentUserIdUnaccesibleException()
        {
        }

        public CurrentUserIdUnaccesibleException(string message) : base(message)
        {
        }

        public CurrentUserIdUnaccesibleException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected CurrentUserIdUnaccesibleException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}