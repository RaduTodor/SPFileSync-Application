using Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace DataAccessLayer
{
    public class MetadataGetter
    {
        private static readonly XNamespace metadataBaseNamespace = "http://www.w3.org/2005/Atom";

        private static readonly XNamespace mNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        private static readonly XNamespace dNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        public static DateTime GetModifiedDateOfItem(ConnectionConfiguration connectionConfiguration, string fileUrl)
        {
            fileUrl = fileUrl.Replace("%20", " ");
            string listTitle = ParseURLParentDirectory(fileUrl);
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(connectionConfiguration.Connection.Uri.AbsoluteUri +
                $"_api/Web/lists/getbytitle('{listTitle}')/items?" +
                $"$select=Modified" +
                $"&$filter=FileRef eq '{fileUrl}'");
            endpointRequest.Method = "GET";
            endpointRequest.Credentials = new NetworkCredential(connectionConfiguration.Connection.Credentials.UserName, connectionConfiguration.Connection.Credentials.Password);
            endpointRequest.Accept = "application/xml;odata=verbose";
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            string result = "";
            using (System.IO.Stream stream = endpointResponse.GetResponseStream())
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                    return GetModifiedDateInResponse(result);
                }
            }
        }

        public static List<string> GetCurrentUserUrls(ConnectionConfiguration connectionConfiguration)
        {
            List<string> allUrlsOfCurrentUser = new List<string>();
            foreach (ListWithColumnsName listWithColumnsName in connectionConfiguration.ListsWithColumnsNames)
            {
                HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(connectionConfiguration.Connection.Uri.AbsoluteUri +
                    $"_api/Web/lists/getbytitle('{listWithColumnsName.ListName}')/items?" +
                    $"$select={listWithColumnsName.UrlColumnName}" +
                    $"&$filter={listWithColumnsName.UserColumnName} eq '{connectionConfiguration.Connection.GetCurrentUserName()}'");
                endpointRequest.Method = "GET";
                endpointRequest.Credentials = new NetworkCredential(connectionConfiguration.Connection.Credentials.UserName, connectionConfiguration.Connection.Credentials.Password);
                endpointRequest.Accept = "application/xml;odata=verbose";
                HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
                string result = "";
                using (System.IO.Stream stream = endpointResponse.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                    {
                        result = sr.ReadToEnd().Trim();
                        allUrlsOfCurrentUser.AddRange(GetAllUrlsInResponse(result, listWithColumnsName.UrlColumnName));
                    }
                }
            }
            return allUrlsOfCurrentUser;
        }

        public static List<string> GetAllUrlsInResponse(string xmlString, string urlColumnName)
        {
            var elements = XElement.Parse(xmlString);
            var result = from entryBody in elements.Elements(metadataBaseNamespace + "entry")
                         from contentBody in entryBody.Elements(metadataBaseNamespace + "content")
                         from propertiesBody in contentBody.Elements(mNamespace + "properties")
                         from urlNameBody in propertiesBody.Elements(dNamespace + urlColumnName)
                         from url in urlNameBody.Elements(dNamespace + "Url")
                         select url;
            List<string> Urls = new List<string>();
            foreach (var element in result)
            {
                Urls.Add(element.Value);
            }
            return Urls;
        }

        public static DateTime GetModifiedDateInResponse(string xmlString)
        {
            var elements = XElement.Parse(xmlString);
            var result = from entryBody in elements.Elements(metadataBaseNamespace + "entry")
                         from contentBody in entryBody.Elements(metadataBaseNamespace + "content")
                         from propertiesBody in contentBody.Elements(mNamespace + "properties")
                         from modifiedDate in propertiesBody.Elements(dNamespace + "Modified")
                         select modifiedDate;
            return Convert.ToDateTime(result.First().Value);
        }

        public static string ParseURLParentDirectory(string url)
        {
            Uri uri = new Uri(url);
            uri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            string final = uri.Segments[3];
            final = final.Remove(final.Length - 1);
            return final.Replace("%20", " ");
        }
    }
}
