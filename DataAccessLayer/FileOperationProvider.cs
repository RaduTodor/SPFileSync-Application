namespace DataAccessLayer
{
    using Common.Exceptions;
    using Configuration;
    using System;
    using System.IO;
    using Common.Helpers;
    using System.Net;
    using Common.Constants;

    /// <summary>
    /// From given ConnectionConfiguration can access specific files 
    /// </summary>
    public class FileOperationProvider
    {
        private ConnectionConfiguration ConnectionConfiguration { get; }

        public FileOperationProvider(ConnectionConfiguration configuration)
        {
            ConnectionConfiguration = configuration;
        }

        /// <summary>
        /// Writes the file from <paramref name="url"/> in a directory given by <paramref name="directoryPath"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="directoryPath"></param>
        /// <param name="exceptionHandler"></param>
        public void Download(string url, string directoryPath, EventHandler<Exception> exceptionHandler)
        {
            try
            {
                using (var response = CreateDownloadRequest(url).GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var downloadedFilePath = Path.Combine(directoryPath, ParsingHelpers.ParseUrlFileName(url));
                        using (var fileStream = new FileStream(downloadedFilePath, FileMode.Create))
                        {
                            var read = new byte[DataAccessLayerConstants.StreamReadBufferBytesDimension];
                            if (stream != null)
                            {
                                var count = stream.Read(read, 0, read.Length);
                                while (count > 0)
                                {
                                    fileStream.Write(read, 0, count);
                                    count = stream.Read(read, 0, read.Length);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                DownloadFileException downloadFileExceptionexception =
                    new DownloadFileException(DefaultExceptionMessages.FileDownloadExceptionMessage, exception);
                MyLogger.Logger.Error(downloadFileExceptionexception);
                exceptionHandler?.Invoke(this, downloadFileExceptionexception);
            }
        }

        /// <summary>
        /// Gets the file from given sharepoint <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebRequest CreateDownloadRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var credentials = ConnectionConfiguration.Connection.Credentials;
            request.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
            request.Timeout = DataAccessLayerConstants.WebRequestTimeoutValue;
            request.AllowWriteStreamBuffering = false;
            return request;
        }
    }
}
