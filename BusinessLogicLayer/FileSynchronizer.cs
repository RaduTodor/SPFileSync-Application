namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using System.IO;
    using System.Runtime.Remoting.Channels;
    using Common.Constants;
    using Common.Helpers;
    using Configuration;
    using DataAccessLayer;
    using Common.ApplicationEnums;


    /// <summary>
    /// The FileSynchronizer instance can Synchronize (check and download) all referenceListItems (theirs urls) from a current ConnectionConfiguration
    /// </summary>
    public class FileSynchronizer
    {
        private const string Backslash = "\\";

        private FileOperationProvider FileOperationProvider { get; }

        private BaseListReferenceProvider ListReferenceProvider { get; }

        public event EventHandler<Exception> ExceptionUpdate;

        public FileSynchronizer(ConnectionConfiguration configuration, ListReferenceProviderType type)
        {
            FileOperationProvider = new FileOperationProvider(configuration);
            ListReferenceProvider = OperationsFactory.GetOperations(type);
            ListReferenceProvider.ConnectionConfiguration = configuration;
        }



        /// <summary>
        /// Gets sharepoint listReferenceItems, compares with the local infos, downloads if case and writes the modified infos locally
        /// </summary>
        public void Synchronize()
        {
            try
            {
                var spData = GetUserUrlsWithDate();
                var currentData = CsvMetadataFileManipulator.ReadMetadata<MetadataModel>(string.Format(HelpersConstants.MetadataFileLocation,
                        ListReferenceProvider.ConnectionConfiguration.DirectoryPath,
                        ListReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()),
                    ListReferenceProvider.ConnectionConfiguration.DirectoryPath);
                foreach (var model in spData)
                {
                    EnsureFile(model, currentData, ExceptionUpdate);
                }
                CsvMetadataFileManipulator.WriteMetadata(string.Format(HelpersConstants.MetadataFileLocation,
                        ListReferenceProvider.ConnectionConfiguration.DirectoryPath,
                        ListReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()),
                    currentData);
            }
            catch (Exception exception)
            {
                MyLogger.Logger.Error(exception);
                ExceptionUpdate?.Invoke(this, exception);
            }

        }

        /// <summary>
        /// Checks if given MetadataModel is in given list and if it is compares the ModifiedDate, if so calls Download on it
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentData"></param>
        /// <param name="exceptionHandler"></param>
        private void EnsureFile(MetadataModel model, List<MetadataModel> currentData, EventHandler<Exception> exceptionHandler)
        {
            var match = currentData.FirstOrDefault(x => x.Url == model.Url);
            if (match != null && match.ModifiedDate < model.ModifiedDate)
            {
                FileOperationProvider.Download(match.Url, ListReferenceProvider.ConnectionConfiguration.DirectoryPath, exceptionHandler);
                currentData.Remove(match);
                currentData.Add(model);
            }
            else
            {
                if (!File.Exists(ListReferenceProvider.ConnectionConfiguration.DirectoryPath + Backslash + ParsingHelpers.ParseUrlFileName(model.Url)))
                {
                    FileOperationProvider.Download(model.Url, ListReferenceProvider.ConnectionConfiguration.DirectoryPath, exceptionHandler);
                    currentData.Add(model);
                }
            }
        }

        /// <summary>
        /// Gets all MetadataModel instances from current configuration sharepoint
        /// </summary>
        /// <returns></returns>
        private List<MetadataModel> GetUserUrlsWithDate()
        {
            var metadatas = new List<MetadataModel>();
            List<string> userUrLs = ListReferenceProvider.GetCurrentUserUrls();
            foreach (var url in userUrLs)
            {
                DateTime dateTime = ListReferenceProvider.GetMetadataItem(url);
                metadatas.Add(new MetadataModel { Url = url, ModifiedDate = dateTime });
            }
            return metadatas;
        }
    }
}
