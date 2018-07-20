namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class MetadataReadException : BaseLoggingException
    {
        public MetadataReadException()
        {
        }

        public MetadataReadException(string message) : base(message)
        {
        }

        public MetadataReadException(string message, Exception innerException) : base(message, innerException)
        {
            
        }

        protected MetadataReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
