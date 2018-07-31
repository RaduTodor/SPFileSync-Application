namespace DataAccessLayer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Common.Constants;
    using Common.Exceptions;
    using Common.Helpers;
    using Configuration;
    using Models;

    /// <summary>
    ///     From given ConnectionConfiguration can access specific files
    /// </summary>
    //TODO [CR RT]: Remove unused namespaces
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
        /// <param name="internetAccessException"></param>
        public void Download(string url, string directoryPath, EventHandler<Exception> exceptionHandler, bool update, EventHandler<Exception> internetAccessException)
        {
            var fileName = ParsingHelpers.ParseUrlFileName(url);
            var downloadedFilePath = Path.Combine(directoryPath, fileName);
            try
            {
                using (var response = CreateDownloadRequest(url).GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
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
                            LoggerManager.Logger.Trace(string.Format(DefaultTraceMessages.FileDownloadSuccessful,
                                fileName,
                                url,
                                directoryPath));
                        }
                        else
                        {
                            string updateMessage = string.Format(DefaultTraceMessages.FileUpdateSuccessful,
                                fileName,
                                url,
                                directoryPath);
                            LoggerManager.Logger.Trace(updateMessage);
                            NotifyUI notifyUi = new NotifyUI();
                            notifyUi.NotifyUserWithTrayBarBalloon(ConfigurationMessages.FileUpdatedTitle, updateMessage);
                            AddUpdatedFileInformations(url, fileName, downloadedFilePath);
                        }
                    }
                }
            }
            catch (System.Net.WebException exception)
            {
                CatchDownloadException(exception, internetAccessException);
            }
            catch (System.IO.IOException exception)
            {
                File.Delete(downloadedFilePath);
                CatchDownloadException(exception, internetAccessException);
            }
            catch (Exception exception)
            {
                Exception downloadFileExceptionexception =
                    new DownloadFileException(exception.Message, exception);
                LoggerManager.Logger.Debug(downloadFileExceptionexception,
                    string.Format(DefaultExceptionMessages.FileDownloadExceptionMessage, url));
                exceptionHandler?.Invoke(this, downloadFileExceptionexception);
            }
        }

        private void AddUpdatedFileInformations(string url, string fileName, string downloadedFilePath)
        {
            UpdatedFilesModel updatedFilesModel = UpdatedFilesModel.Instance;
            int existentFilePosition = updatedFilesModel.UpdatedFilesUrl.FindIndex(file => file == url);
            if (existentFilePosition != -1)
            {
                updatedFilesModel.RemoveElementAt(existentFilePosition);
            }
            //TODO [CR RT]: Use a model instead multiple lists
            updatedFilesModel.UpdatedFilesUrl.Add(url);
            updatedFilesModel.UpdatedFilesName.Add(fileName);
            updatedFilesModel.UpdatedFilesLocation.Add(downloadedFilePath);
            updatedFilesModel.UpdatedFilesUpdateMoment.Add(DateTime.Now);
        }

        private void CatchDownloadException(Exception exception, EventHandler<Exception> internetAccessException)
        {
            Exception currentException =
                new NoInternetAccessException(exception.Message, exception);
            LoggerManager.Logger.Error(currentException, string.Format(
                DefaultExceptionMessages.NoInternetAccessExceptionMessage,
                DataAccessLayerConstants.SyncRetryInterval));
            internetAccessException?.Invoke(this, currentException);
            throw currentException;
        }

        /// <summary>
        ///     Gets the file from given sharepoint <paramref name="url" />
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