namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class CreateFileException : BaseLoggingException
    {
        public CreateFileException()
        {
        }

        public CreateFileException(string message) : base(message)
        {
        }

        public CreateFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CreateFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
