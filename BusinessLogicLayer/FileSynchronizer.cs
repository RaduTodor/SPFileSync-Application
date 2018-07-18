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

    //TODO [CR RT]: Add class and methods documentation

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

        public void Synchronize()
        {
            var spData = GetUserUrlsWithDate();
            var currentData = CsvFileManipulator.ReadMetadata<MetadataModel>(string.Format(HelpersConstant.MetadataFileLocation,
                listReferenceProvider.ConnectionConfiguration.DirectoryPath,
                listReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()),
                listReferenceProvider);
            foreach (var model in spData)
            {
                EnsureFile(model, currentData);
            }
            CsvFileManipulator.WriteMetadata(string.Format(HelpersConstant.MetadataFileLocation,
                listReferenceProvider.ConnectionConfiguration.DirectoryPath,
                listReferenceProvider.ConnectionConfiguration.Connection.GetSharepointIdentifier()), 
                currentData);
        }

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
