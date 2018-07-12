using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.IO;
using System.Text.RegularExpressions;
using CsvHelper;

namespace BusinessLogicLayer
{
    public class FileSynchronizer
    {
        public DataAccessOperations DataAccessOperations { get; set; }

        public void Synchronize()
        {
            Dictionary<string, DateTime> spData = GetUserUrlsWithDate();
            Dictionary<string, DateTime> currentData = ReadMetadata(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "\\data.csv");
            foreach(KeyValuePair<string,DateTime> pair in spData)
            {
                KeyValuePair<string, DateTime> match = currentData.FirstOrDefault(x => x.Key == pair.Key);
                if (match.Key!=null && match.Value > pair.Value)
                {
                    DataAccessOperations.FilesGetter.DownloadFile(match.Key, DataAccessOperations.ConnectionConfiguration.DirectoryPath);
                    currentData.Remove(match.Key);
                    currentData.Add(pair.Key, pair.Value);
                }
                else
                {
                    DataAccessOperations.FilesGetter.DownloadFile(pair.Key, DataAccessOperations.ConnectionConfiguration.DirectoryPath);
                    currentData.Add(pair.Key, pair.Value);
                }
            }
            WriteMetadata(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "\\data.csv", currentData);
        }

        public Dictionary<string, DateTime> GetUserUrlsWithDate()
        {
            List<ListItem> userListItems = ListOperations.GetAllUserItems(DataAccessOperations);
            List<string> userURLs = new List<string>();
            foreach (ListItem item in userListItems)
            {
               
                userURLs.Add(SPItemManipulator.GetValueURL(item, "URL"));
            }
            Dictionary<string, DateTime> metadata = new Dictionary<string, DateTime>();
            foreach (string url in userURLs)
            {
                DateTime dateTime= FindModifiedDateTime(DataAccessOperations.Operations.GetMetadataItem(url));
                metadata.Add(url, dateTime);
            }
            return metadata;
        }

        static public void WriteMetadata(string filePath, Dictionary<string, DateTime> urlsDate)
        {
            using (var csv = new CsvWriter(System.IO.File.CreateText(filePath)))
            {
                csv.WriteRecords(urlsDate);
            }
        }

        public static Dictionary<string, DateTime> ReadMetadata(string filePath)
        {
            var file=System.IO.File.OpenText(filePath);
            var csv = new CsvReader(file);
            csv.Configuration.IgnoreBlankLines = true;
            csv.Read();
            IDictionary<string, DateTime> records = null;
            try
            {
                records = csv.GetRecords<dynamic>() as IDictionary<string, DateTime>;
            }
            catch(Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    records = new Dictionary<string, DateTime>();
                }
            }
            file.Close();
            return (Dictionary<string,DateTime>)records;
        }


        public static DateTime FindModifiedDateTime(string json)
        {
            string result= json.Substring(json.LastIndexOf("201"),json.Length- json.LastIndexOf("201"));
            result = result.Substring(0, result.Length - 6); //MAGIC NUMBER EVERYBODY
            return Convert.ToDateTime(result);
        }

        public static string ParseURLParentDirectory(string url)
        {
            Uri uri = new Uri(url);
            uri=new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            return uri.AbsoluteUri;
        }
        
        public DateTime GetFileModifyDate(string fileName)
        {
            return System.IO.File.GetLastWriteTime(DataAccessOperations.ConnectionConfiguration.DirectoryPath + "/" + fileName);
        }
    }
}
