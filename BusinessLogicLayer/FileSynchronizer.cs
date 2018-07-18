using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace BusinessLogicLayer
{
    //TODO [CR RT]: Make DataAccessOperations private and make ctor for initializing it. This member is not used from outside of the class, except for initialization -> Incapsulation issue if it is let public.

    //TODO [CR RT]: Add class and methods documentation

    public class FileSynchronizer
    {
        public DataAccessOperations DataAccessOperations { get; set; }

        public void Synchronize()
        {
            List<MetadataModel> spData = GetUserUrlsWithDate();
            //TODO [CR RT]: Use string.Format
            List<MetadataModel> currentData = CsvFileManipulator.ReadMetadata<MetadataModel>(DataAccessOperations.ConnectionConfiguration.DirectoryPath +
                $"\\data-{DataAccessOperations.ConnectionConfiguration.Connection.GetSharepointIdentifier()}.csv",DataAccessOperations);
            foreach (MetadataModel model in spData)
            {
                //TODO [CR RT]: Extract to new method named e.g. EnsureFile
                MetadataModel match = currentData.FirstOrDefault(x => x.Url == model.Url);
                if (match != null && match.ModifiedDate < model.ModifiedDate)
                {
                    DataAccessOperations.FilesGetter.DownloadFile(match.Url, DataAccessOperations.ConnectionConfiguration.DirectoryPath);
                    currentData.Remove(match);
                    currentData.Add(model);
                }
                else
                {
                    if (!System.IO.File.Exists(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "\\" + FilesGetter.ParseURLFileName(model.Url)))
                    {
                        DataAccessOperations.FilesGetter.DownloadFile(model.Url, DataAccessOperations.ConnectionConfiguration.DirectoryPath);
                        currentData.Add(model);
                    }
                }
            }
            CsvFileManipulator.WriteMetadata(DataAccessOperations.ConnectionConfiguration.DirectoryPath +
                $"\\data-{DataAccessOperations.ConnectionConfiguration.Connection.GetSharepointIdentifier()}.csv", currentData);
        }

        //TODO [CR RT]: Let methods to be public just when realy needed
        public List<MetadataModel> GetUserUrlsWithDate()
        {
            List<MetadataModel> metadatas = new List<MetadataModel>();
            List<string> userURLs = DataAccessOperations.Operations.GetCurrentUserUrls();
            foreach (string url in userURLs)
            {
                DateTime dateTime = DataAccessOperations.Operations.GetMetadataItem(url);
                metadatas.Add(new MetadataModel { Url = url, ModifiedDate = dateTime });
            }
            return metadatas;
        }
    }
}
