namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class CsomOperationException : BaseLoggingException
    {
        public CsomOperationException()
        {
        }

        public CsomOperationException(string message) : base(message)
        {
        }

        public CsomOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CsomOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}