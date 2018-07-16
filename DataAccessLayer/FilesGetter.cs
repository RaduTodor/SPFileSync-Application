using Configuration;
using Microsoft.SharePoint.Client;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DataAccessLayer
{
    public class FilesGetter
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        private static void DownloadAFile(ListItem item, string targetPath)
        {
            var ctx = (ClientContext)item.Context;
            var fileRef = (string)item["FileRef"];
            var fileName = System.IO.Path.GetFileName(fileRef);
            var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctx, fileRef);
            var filePath = Path.Combine(targetPath, fileName);
            using (var fileStream = System.IO.File.Create(filePath))
            {
                fileInfo.Stream.CopyTo(fileStream);
            }
        }

        public string DownloadFile(string url, string directoryPath)
        {      
            string serverTempdocPath = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var credentials = ConnectionConfiguration.Connection.Credentials;

                request.Credentials = credentials;
                request.Timeout = 20000;
                request.AllowWriteStreamBuffering = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        serverTempdocPath = Path.Combine(directoryPath, ParseURLFileName(url));
                        using (FileStream fs = new FileStream(serverTempdocPath, FileMode.Create))
                        {
                            byte[] read = new byte[256];
                            int count = stream.Read(read, 0, read.Length);
                            while (count > 0)
                            {
                                fs.Write(read, 0, count);
                                count = stream.Read(read, 0, read.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return "";
            }
            return serverTempdocPath;
        }

        public static string ParseURLFileName(string url)
        {
            url=url.Replace("%20", " ");
            return url.Split('/').Last();
        }
        #region CommentedCode
        //public void DownloadListItems(string listName)
        //{
        //    using (var ctx = ConnectionConfiguration.Connection.SharePointResult())
        //    {
        //        var qry = new CamlQuery();
        //        qry.ViewXml = "<View Scope='RecursiveAll'>" +
        //                                 "<Query>" +
        //                                     "<Where>" +
        //                                           "<Eq>" +
        //                                                "<FieldRef Name='FSObjType' />" +
        //                                                "<Value Type='Integer'>0</Value>" +
        //                                           "</Eq>" +
        //                                    "</Where>" +
        //                                  "</Query>" +
        //                               "</View>";

        //        var sourceList = ctx.Web.Lists.GetByTitle(listName);
        //        var items = sourceList.GetItems(qry);
        //        ctx.Load(items);
        //        ctx.ExecuteQuery();
        //        foreach (var item in items)
        //        {
        //            var curPath = ConfigurationManager.AppSettings["DirectoryPath"] + System.IO.Path.GetDirectoryName((string)item["FileRef"]);
        //            Directory.CreateDirectory(curPath);
        //            DownloadAFile(item, curPath);
        //        }
        //    }
        //}
        #endregion
    }
}
