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
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Remove static from methods. Initialize connectionConfiguration from ctor, make it private
    //TODO [CR RT]: Extract constants
    //TODO [CR RT]: Rename class e.g. MetadataProvider.
    //TODO [CR RT]: Extract all Api Urls in a constant class
    //TODO [CR RT]: Exception handling

    public class MetadataGetter
    {
        private static readonly XNamespace metadataBaseNamespace = "http://www.w3.org/2005/Atom";

        private static readonly XNamespace mNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        private static readonly XNamespace dNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        //TODO [CR RT]: Extract duplicate code from GetModifiedDateOfItem and GetCurrentUserUrls (request header build step). Keep just the particular logic in the methods.

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

        //TODO [CR RT]: Make it private
        //TODO [CR RT]: Rename Urls ->urls
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

        //TODO [CR RT]: Make it private
        public static DateTime GetModifiedDateInResponse(string xmlString)
        {
            var elements = XElement.Parse(xmlString);
            //TODO [CR RT]: use string.Format
            var result = from entryBody in elements.Elements(metadataBaseNamespace + "entry")
                         from contentBody in entryBody.Elements(metadataBaseNamespace + "content")
                         from propertiesBody in contentBody.Elements(mNamespace + "properties")
                         from modifiedDate in propertiesBody.Elements(dNamespace + "Modified")
                         select modifiedDate;
            return Convert.ToDateTime(result.First().Value);
        }

        //TODO [CR RT]: Make it private
        //TODO [CR RT]: Ranem final; give intuitive naming
        public static string ParseURLParentDirectory(string url)
        {
            Uri uri = new Uri(url);
            //TODO [CR RT]: Extract new Uri() parameter into a variable with explicit naming
            uri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));

            //TODO [CR RT]: 3->Magic number -> should say what represents
            string final = uri.Segments[3];
            final = final.Remove(final.Length - 1);
            return final.Replace("%20", " ");
        }
    }
}
