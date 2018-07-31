namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Common.ApplicationEnums;
    using Common.Constants;
    using Common.Helpers;
    using Configuration;
    using DataAccessLayer;
    using IWshRuntimeLibrary;
    using Microsoft.WindowsAPICodePack.Shell;
    using Models;
    using File = System.IO.File;

    /// <summary>
    ///     The FileSynchronizer instance can Synchronize (check and download) all referenceListItems (theirs urls) from a
    ///     current ConnectionConfiguration
    /// </summary>
    public class FileSynchronizer
    {
        private const string Backslash = "\\";

        public FileSynchronizer(ConnectionConfiguration configuration, ListReferenceProviderType type, int count)
        {
            FileOperationProvider = new FileOperationProvider(configuration);
            ListReferenceProvider = OperationsFactory.GetOperations(type);
            ListReferenceProvider.ConnectionConfiguration = configuration;
            ConfigurationNumber = count;
        }

        private FileOperationProvider FileOperationProvider { get; }

        private BaseListReferenceProvider ListReferenceProvider { get; }

        private int ConfigurationNumber { get; set; }

        public event EventHandler<Exception> ExceptionUpdate;

        public event EventHandler<Exception> InternetAccessException;

        public event EventHandler<int> ProgressUpdate;

        /// <summary>
        ///     Gets sharepoint listReferenceItems, compares with the local infos, downloads if case and writes the modified infos
        ///     locally
        /// </summary>
        public void Synchronize()
        {
            try
            {
                var spData = GetUserUrlsWithDate();
                var currentData = CsvMetadataFileManipulator.ReadMetadata<MetadataModel>(
                    Directory.GetCurrentDirectory() + string.Format(
                        HelpersConstants.MetadataFileLocation,
                        ListReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()),
                    Directory.GetCurrentDirectory() + HelpersConstants.ParentDirectory);
                foreach (var model in spData)
                {
                    if (model.ModifiedDate != DateTime.MinValue)
                    {
                        EnsureFile(model, currentData);
                    }
                }
                CsvMetadataFileManipulator.WriteMetadata(Directory.GetCurrentDirectory() + string.Format(
                                                             HelpersConstants.MetadataFileLocation,
                                                             ListReferenceProvider.ConnectionConfiguration.Connection
                                                                 .GetSharepointIdentifier()),
                    currentData);
                ProgressUpdate?.Invoke(this, ConfigurationNumber);
            }
            catch (Exception exception)
            {
                LoggerManager.Logger.Error(exception);
                ExceptionUpdate?.Invoke(this, exception);
                ProgressUpdate?.Invoke(this, ConfigurationNumber);
            }
        }
        /// <summary>
        ///     Checks if given MetadataModel is in given list and if it is compares the ModifiedDate, if so calls Download on it
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentData"></param>
        private void EnsureFile(MetadataModel model, List<MetadataModel> currentData)
        {
            var match = currentData.FirstOrDefault(x => x.Url == model.Url);
            string fileName = ParsingHelpers.ParseUrlFileName(model.Url);
            string directoryPath = ListReferenceProvider.ConnectionConfiguration.DirectoryPath;
            string filePath = string.Format(HelpersConstants.FilePath,
                directoryPath,
                fileName);
            if (match != null && match.ModifiedDate < model.ModifiedDate)
            {
                DownloadFileAndAddMetadata(true, currentData, model);
                currentData.Remove(match);
            }
            else
            {
                
                if (!File.Exists(filePath))
                {
                    DownloadFileAndAddMetadata(false, currentData, model);
                    if (match != null)
                    {
                        currentData.Remove(match);
                    }
                }
                else
                {
                    if (match == null) currentData.Add(model);
                }
            }
        }

        private void DownloadFileAndAddMetadata(bool updated, List<MetadataModel> currentData, MetadataModel model)
        {
            FileOperationProvider.Download(model.Url,
                ListReferenceProvider.ConnectionConfiguration.DirectoryPath, ExceptionUpdate, updated, InternetAccessException);
            currentData.Add(model);
        }

        /// <summary>
        ///     Gets all MetadataModel instances from current configuration sharepoint
        /// </summary>
        /// <returns></returns>
        private List<MetadataModel> GetUserUrlsWithDate()
        {
            var metadatas = new List<MetadataModel>();
            var userUrLs = ListReferenceProvider.GetCurrentUserUrls(ExceptionUpdate, InternetAccessException);
            foreach (var url in userUrLs)
            {
                var dateTime = ListReferenceProvider.GetMetadataItem(url, InternetAccessException);
                metadatas.Add(new MetadataModel { Url = url, ModifiedDate = dateTime });
            }

            return metadatas;
        }

    }
}