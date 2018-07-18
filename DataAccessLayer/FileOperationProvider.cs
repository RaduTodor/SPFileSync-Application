namespace DataAccessLayer
{
    using Configuration;
    using System;
    using System.IO;
    using Common.Helpers;
    using System.Net;
    using Common.Constants;
    //TODO [CR RT]: Add class and methods documentation
    public class FileOperationProvider
    {
        private ConnectionConfiguration ConnectionConfiguration { get; }

        public FileOperationProvider(ConnectionConfiguration configuration)
        {
            ConnectionConfiguration = configuration;
        }

        public void Download(string url, string directoryPath)
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
            catch (Exception exc)
            {
                //TODO [CR RT]: Log exception in document log
            }
        }

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
