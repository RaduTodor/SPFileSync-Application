﻿using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using System.IO;
using CsvHelper;
using Models;
using System.Globalization;
using System.Text;

namespace BusinessLogicLayer
{
    public class FileSynchronizer
    {
        public DataAccessOperations DataAccessOperations { get; set; }

        public void Synchronize()
        {
            List<MetadataModel> spData = GetUserUrlsWithDate();
            List<MetadataModel> currentData = ReadMetadata<MetadataModel>(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "\\data.csv");
            foreach (MetadataModel model in spData)
            {
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
            WriteMetadata(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "\\data.csv", currentData);
        }

        public List<MetadataModel> GetUserUrlsWithDate()
        {
            List<ListItem> userListItems = ListOperations.GetAllUserItems(DataAccessOperations);
            List<string> userURLs = new List<string>();
            foreach (ListItem item in userListItems)
            {

                userURLs.Add(SPItemManipulator.GetValueURL(item, "URL"));
            }
            List<MetadataModel> metadata = new List<MetadataModel>();
            foreach (string url in userURLs)
            {
                DateTime dateTime = FindModifiedDateTime(DataAccessOperations.Operations.GetMetadataItem(url));
                metadata.Add(new MetadataModel { Url = url, ModifiedDate = dateTime });
            }
            return metadata;
        }

        public void WriteMetadata<T>(string filePath, List<T> list)
        {
            using (var csv = new CsvWriter(System.IO.File.CreateText(filePath)))
            {
                csv.Configuration.Delimiter = ";";
                csv.WriteRecords(list);
            }
        }

        public List<T> ReadMetadata<T>(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                StreamWriter streamWriter = System.IO.File.CreateText(filePath);
                streamWriter.Close();
            }
            List<T> records = new List<T>();
            using (var file = System.IO.File.OpenText(filePath))
            {
                using (var csv = new CsvReader(file))
                {
                    csv.Configuration.RegisterClassMap<MetadataModelCsvMap>();
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.Encoding = Encoding.UTF8;
                    try
                    {
                        records = csv.GetRecords<T>().ToList();
                    }
                    catch (Exception ex)
                    {
                        if (ex is NullReferenceException)
                        {
                            records = new List<T>();
                        }
                    }
                }
            }
            return records;
        }

        public static DateTime FindModifiedDateTime(string json)
        {
            string result = json.Substring(json.LastIndexOf("201"), json.Length - json.LastIndexOf("201"));
            result = result.Substring(0, result.Length - 6); //MAGIC NUMBER EVERYBODY
            return Convert.ToDateTime(result);
        }

        public static string ParseURLParentDirectory(string url)
        {
            Uri uri = new Uri(url);
            uri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            return uri.AbsoluteUri;
        }

        public DateTime GetFileModifyDate(string fileName)
        {
            return System.IO.File.GetLastWriteTime(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "/" + fileName);
        }
    }
}
