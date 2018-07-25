namespace DataAccessLayer
{
    using System;
    using System.IO;
    using System.Net;
    using Common.Constants;
    using Common.Exceptions;
    using Common.Helpers;
    using Configuration;

    /// <summary>
    ///     From given ConnectionConfiguration can access specific files
    /// </summary>
    public class FileOperationProvider
    {
        public FileOperationProvider(ConnectionConfiguration configuration)
        {
            ConnectionConfiguration = configuration;
        }

        private ConnectionConfiguration ConnectionConfiguration { get; }

        /// <summary>
        ///     Writes the file from <paramref name="url" /> in a directory given by <paramref name="directoryPath" />
        /// </summary>
        /// <param name="url"></param>
        /// <param name="directoryPath"></param>
        /// <param name="exceptionHandler"></param>
        /// <param name="update"></param>
        public void Download(string url, string directoryPath, EventHandler<Exception> exceptionHandler, bool update)
        {
            try
            {
                using (var response = CreateDownloadRequest(url).GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        string fileName = ParsingHelpers.ParseUrlFileName(url);
                        var downloadedFilePath = Path.Combine(directoryPath, fileName);
                        FileEditingHelper.CreateAccesibleFile(downloadedFilePath, directoryPath);
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

                        if (!update)
                        {
                            MyLogger.Logger.Debug(String.Format(DefaultDebugMessages.FileDownloadSuccessful, fileName, url,
                                directoryPath));
                        }
                        else
                        {
                            MyLogger.Logger.Debug(String.Format(DefaultDebugMessages.FileUpdateSuccessful, fileName, url,
                                directoryPath));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Exception downloadFileExceptionexception =
                    new DownloadFileException(exception.Message, exception);
                MyLogger.Logger.Error(downloadFileExceptionexception, string.Format(DefaultExceptionMessages.FileDownloadExceptionMessage, url));
                exceptionHandler?.Invoke(this, downloadFileExceptionexception);
            }
        }

        /// <summary>
        ///     Gets the file from given sharepoint <paramref name="url" />
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebRequest CreateDownloadRequest(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            var credentials = ConnectionConfiguration.Connection.Credentials;
            request.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
            request.Timeout = DataAccessLayerConstants.WebRequestTimeoutValue;
            request.AllowWriteStreamBuffering = false;
            return request;
        }
    }
}