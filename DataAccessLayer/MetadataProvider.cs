namespace DataAccessLayer
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using Common.Constants;

    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Extract constants
    //TODO [CR RT]: Exception handling

    public class MetadataProvider
    {
        private ConnectionConfiguration connectionConfiguration { get; }

        public MetadataProvider(ConnectionConfiguration configuration)
        {
            connectionConfiguration = configuration;
        }

        private void AddGetHeadersToRequest(HttpWebRequest request)
        {
            request.Method = "GET";
            request.Credentials = new NetworkCredential(connectionConfiguration.Connection.Credentials.UserName, connectionConfiguration.Connection.Credentials.Password);
            request.Accept = "application/xml;odata=verbose";
        }

        public DateTime GetModifiedDateOfItem(string fileUrl)
        {
            fileUrl = fileUrl.Replace("%20", " ");
            var listTitle = ParseUrlParentDirectory(fileUrl);
            var endpointRequest = (HttpWebRequest)WebRequest.Create(connectionConfiguration.Connection.Uri.AbsoluteUri +
                string.Format(QuerryTemplates.ModifiedDateOfUrlApi,listTitle,fileUrl));

            AddGetHeadersToRequest(endpointRequest);
            var endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            using (var stream = endpointResponse.GetResponseStream())
            {
                var result = string.Empty;
                if (stream != null)
                    using (var sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                    {
                        result = sr.ReadToEnd();
                    }
                return GetModifiedDateInResponse(result);
            }
        }

        public List<string> GetCurrentUserUrls()
        {
            var allUrlsOfCurrentUser = new List<string>();
            foreach (var listWithColumnsName in connectionConfiguration.ListsWithColumnsNames)
            {
                var endpointRequest = (HttpWebRequest) WebRequest.Create(
                    connectionConfiguration.Connection.Uri.AbsoluteUri +
                    string.Format(QuerryTemplates.SpecificListItemsOfUserApi, listWithColumnsName.ListName,
                        listWithColumnsName.UrlColumnName, listWithColumnsName.UserColumnName,
                        connectionConfiguration.Connection.GetCurrentUserName()));
                AddGetHeadersToRequest(endpointRequest);
                var endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
                
                using (var stream = endpointResponse.GetResponseStream())
                {
                    if (stream != null)
                        using (var sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                        {
                            var result = sr.ReadToEnd().Trim();
                            allUrlsOfCurrentUser.AddRange(GetAllUrlsInResponse(result,
                                listWithColumnsName.UrlColumnName));
                        }
                }
            }
            return allUrlsOfCurrentUser;
        }

        private List<string> GetAllUrlsInResponse(string xmlString, string urlColumnName)
        {
            var elements = XElement.Parse(xmlString);
            var result = from entryBody in elements.Elements(DataAccessLayerConstants.MetadataBaseNamespace + "entry")
                         from contentBody in entryBody.Elements(DataAccessLayerConstants.MetadataBaseNamespace + "content")
                         from propertiesBody in contentBody.Elements(DataAccessLayerConstants.MNamespace + "properties")
                         from urlNameBody in propertiesBody.Elements(DataAccessLayerConstants.DNamespace + urlColumnName)
                         from url in urlNameBody.Elements(DataAccessLayerConstants.DNamespace + "Url")
                         select url;
            var urls = new List<string>();
            foreach (var element in result)
            {
                urls.Add(element.Value);
            }
            return urls;
        }

        private DateTime GetModifiedDateInResponse(string xmlString)
        {
            var elements = XElement.Parse(xmlString);
            //TODO [CR RT]: use string.Format
            var result = from entryBody in elements.Elements(DataAccessLayerConstants.MetadataBaseNamespace + "entry")
                         from contentBody in entryBody.Elements(DataAccessLayerConstants.MetadataBaseNamespace + "content")
                         from propertiesBody in contentBody.Elements(DataAccessLayerConstants.MNamespace + "properties")
                         from modifiedDate in propertiesBody.Elements(DataAccessLayerConstants.DNamespace + "Modified")
                         select modifiedDate;
            return Convert.ToDateTime(result.First().Value);
        }

        private string ParseUrlParentDirectory(string url)
        {
            var uri = new Uri(url);
            var libraryUri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            var parentDirectory = libraryUri.Segments[DataAccessLayerConstants.LibrarySegmentNumber];
            parentDirectory = parentDirectory.Remove(parentDirectory.Length - 1);
            return parentDirectory.Replace("%20", " ");
        }
    }
}
