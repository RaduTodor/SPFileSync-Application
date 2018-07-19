namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using System.IO;
    using Common.Constants;
    using Common.Helpers;
    using Configuration;
    using DataAccessLayer;

    /// <summary>
    /// The FileSynchronizer instance can Synchronize (check and download) all referenceListItems (theirs urls) from a current ConnectionConfiguration
    /// </summary>
    public class FileSynchronizer
    {
        private FileOperationProvider fileOperationProvider { get; }

        private BaseListReferenceProvider listReferenceProvider { get; }

        public FileSynchronizer(ConnectionConfiguration configuration, ApplicationEnums.ListReferenceProviderType type)
        {
            fileOperationProvider = new FileOperationProvider(configuration);
            listReferenceProvider = OperationsFactory.GetOperations(type);
            listReferenceProvider.ConnectionConfiguration = configuration;
        }

        /// <summary>
        /// Gets sharepoint listReferenceItems, compares with the local infos, downloads if case and writes the modified infos locally
        /// </summary>
        public void Synchronize()
        {
            var spData = GetUserUrlsWithDate();
            var currentData = CsvFileManipulator.ReadMetadata<MetadataModel>(string.Format(HelpersConstant.MetadataFileLocation,
                listReferenceProvider.ConnectionConfiguration.DirectoryPath,
                listReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()),
                listReferenceProvider.ConnectionConfiguration.DirectoryPath);
            foreach (var model in spData)
            {
                EnsureFile(model, currentData);
            }
            CsvFileManipulator.WriteMetadata(string.Format(HelpersConstant.MetadataFileLocation,
                listReferenceProvider.ConnectionConfiguration.DirectoryPath,
                listReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()), 
                currentData);
        }

        /// <summary>
        /// Checks if given MetadataModel is in given list and if it is compares the ModifiedDate, if so calls Download on it
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentData"></param>
        public void EnsureFile(MetadataModel model, List<MetadataModel> currentData)
        {
            var match = currentData.FirstOrDefault(x => x.Url == model.Url);
            if (match != null && match.ModifiedDate < model.ModifiedDate)
            {
                fileOperationProvider.Download(match.Url, listReferenceProvider.ConnectionConfiguration.DirectoryPath);
                currentData.Remove(match);
                currentData.Add(model);
            }
            else
            {
                if (!File.Exists(listReferenceProvider.ConnectionConfiguration.DirectoryPath + "\\" + ParsingHelpers.ParseUrlFileName(model.Url)))
                {
                    fileOperationProvider.Download(model.Url, listReferenceProvider.ConnectionConfiguration.DirectoryPath);
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
            List<string> userUrLs = listReferenceProvider.GetCurrentUserUrls();
            foreach (var url in userUrLs)
            {
                DateTime dateTime = listReferenceProvider.GetMetadataItem(url);
                metadatas.Add(new MetadataModel { Url = url, ModifiedDate = dateTime });
            }
            return metadatas;
        }
    }
}
