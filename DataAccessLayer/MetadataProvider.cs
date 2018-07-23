using Common.Exceptions;

namespace DataAccessLayer
{
    using Common.Helpers;
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using Common.Constants;

    /// <summary>
    /// Can get MetadataModel(Url with ModifiedDate)
    /// </summary>
    public class MetadataProvider
    {
        //TODO [CR RT] : Use capital letters for properties connectionConfigurations - > ConnectionConfigurations
        private ConnectionConfiguration connectionConfiguration { get; }

        private const string Entry = "entry";

        private const string Content = "content";

        private const string Properties = "properties";

        private const string Url = "Url";

        private const string Modified = "Modified";

        public MetadataProvider(ConnectionConfiguration configuration)
        {
            connectionConfiguration = configuration;
        }

        /// <summary>
        /// Adds Credentials, method name and accept form to a request)
        /// </summary>
        /// <param name="request"></param>
        private void AddGetHeadersToRequest(HttpWebRequest request)
        {
            request.Method = RequestHeaderConstants.Get;
            request.Credentials = new NetworkCredential(connectionConfiguration.Connection.Credentials.UserName, connectionConfiguration.Connection.Credentials.Password);
            request.Accept = DataAccessLayerConstants.ContentTypeXml;
        }

        /// <summary>
        /// Sends a request for getting the modification date of an ListItem
        /// Calls GetModifiedDateInResponse
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public DateTime GetModifiedDateOfItem(string fileUrl)
        {
            try
            {
                fileUrl = fileUrl.Replace(HelpersConstants.SpaceReplaceUtfCode, " ");
                var listTitle = ParsingHelpers.ParseUrlParentDirectory(fileUrl);
                var endpointResponse =
                    GetHttpWebResponse(string.Format(ApiConstants.ModifiedDateOfUrlApi, listTitle, fileUrl));

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
            catch (Exception exception)
            {
                throw new GetRequestException(DefaultExceptionMessages.GetRequestExceptionMessage, exception);
            }
        }

        private HttpWebResponse GetHttpWebResponse(string apiResult)
        {
            var endpointRequest = (HttpWebRequest)WebRequest.Create(
                connectionConfiguration.Connection.Uri.AbsoluteUri +
                apiResult);

            AddGetHeadersToRequest(endpointRequest);

            return (HttpWebResponse)endpointRequest.GetResponse();
        }

        /// <summary>
        /// Sends a request for getting all urls from all ReferenceListItems of CurrentUser
        /// Calls GetAllUrlsInResponse
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentUserUrls()
        {
            try
            {
                var allUrlsOfCurrentUser = new List<string>();
                foreach (var listWithColumnsName in connectionConfiguration.ListsWithColumnsNames)
                {
                    var endpointResponse = GetHttpWebResponse(string.Format(ApiConstants.SpecificListItemsOfUserApi,
                        listWithColumnsName.ListName,
                        listWithColumnsName.UrlColumnName, listWithColumnsName.UserColumnName,
                        connectionConfiguration.Connection.GetCurrentUserName()));

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
            catch (Exception exception)
            {
                throw new GetRequestException(DefaultExceptionMessages.GetRequestExceptionMessage,exception);
            }
        }

        /// <summary>
        /// Gets every url from xml response
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="urlColumnName"></param>
        /// <returns></returns>
        private List<string> GetAllUrlsInResponse(string xmlString, string urlColumnName)
        {
            var elements = XElement.Parse(xmlString);
            var result = from entryBody in elements.Elements(DataAccessLayerConstants.MetadataBaseNamespace + Entry)
                         from contentBody in entryBody.Elements(DataAccessLayerConstants.MetadataBaseNamespace + Content)
                         from propertiesBody in contentBody.Elements(DataAccessLayerConstants.MNamespace + Properties)
                         from urlNameBody in propertiesBody.Elements(DataAccessLayerConstants.DNamespace + urlColumnName)
                         from url in urlNameBody.Elements(DataAccessLayerConstants.DNamespace + Url)
                         select url;
            var urls = new List<string>();
            foreach (var element in result)
            {
                urls.Add(element.Value);
            }
            return urls;
        }

        /// <summary>
        /// Gets modified date from xml response
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        private DateTime GetModifiedDateInResponse(string xmlString)
        {
            var elements = XElement.Parse(xmlString);
            var result = from entryBody in elements.Elements(DataAccessLayerConstants.MetadataBaseNamespace + Entry)
                         from contentBody in entryBody.Elements(DataAccessLayerConstants.MetadataBaseNamespace + Content)
                         from propertiesBody in contentBody.Elements(DataAccessLayerConstants.MNamespace + Properties)
                         from modifiedDate in propertiesBody.Elements(DataAccessLayerConstants.DNamespace + Modified)
                         select modifiedDate;
            return Convert.ToDateTime(result.First().Value);
        }
    }
}
