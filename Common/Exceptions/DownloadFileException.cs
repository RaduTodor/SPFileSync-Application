namespace Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class DownloadFileException : BaseLoggingException
    {
        public DownloadFileException()
        {
        }

        public DownloadFileException(string message) : base(message)
        {
        }

        public DownloadFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DownloadFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}