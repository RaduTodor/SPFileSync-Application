namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class ClientContextOperationException : BaseLoggingException
    {
        public ClientContextOperationException()
        {
        }

        public ClientContextOperationException(string message) : base(message)
        {
        }

        public ClientContextOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ClientContextOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
