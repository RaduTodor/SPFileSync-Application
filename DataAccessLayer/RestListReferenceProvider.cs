namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using Common.Constants;
    using Common.Exceptions;
    using Common.Helpers;
    using Models;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     An implementation of BaseListReferenceProvider which uses REST calls technology to implement base methods.
    /// </summary>
    public class RestListReferenceProvider : BaseListReferenceProvider
    {
        private const string StarAll = "*";

        private const string F = "f";

        /// <summary>
        ///     Gets the Digest Value needed for authentication
        /// </summary>
        /// <returns></returns>
        private string RequestFormDigest()
        {
            var webClient = BuildWebClientWithHeader();
            var url = ConnectionConfiguration.Connection.Uri + ApiConstants.ContextInfo;
            var endpointUri = new Uri(url);
            var result = webClient.UploadString(endpointUri, RequestHeaderConstants.Post);
            var token = JToken.Parse(result);
            if (token != null)
                return token[DataAccessLayerConstants.JTokenFirstLayer][DataAccessLayerConstants.JTokenSecondLayer][
                    DataAccessLayerConstants.JTokenThirdLayer].ToString();

            return string.Empty;
        }

        /// <summary>
        ///     Build HttpRequestHeader needed for  Post and Merge operation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="operationType"></param>
        private void BuildHttpRequestHeader(HttpWebRequest request, string operationType)
        {
            request.Method = RequestHeaderConstants.Post;
            request.Accept = DataAccessLayerConstants.ContentTypeJson;
            request.ContentType = DataAccessLayerConstants.ContentTypeJson;
            request.Headers.Add(RequestHeaderConstants.Method, operationType);
            if (operationType == RequestHeaderConstants.Merge)
                request.Headers.Add(RequestHeaderConstants.IfMatch, StarAll);
            request.Headers.Add(RequestHeaderConstants.Digest, RequestFormDigest());
        }

        /// <summary>
        ///     Build WebClientHeader needed for Remove operation and fost GetDigestValue method
        /// </summary>
        /// <returns></returns>
        private WebClient BuildWebClientWithHeader()
        {
            var webClient = new WebClient
            {
                Credentials = new NetworkCredential(ConnectionConfiguration.GetUserName(),
                    ConnectionConfiguration.GetPassword())
            };
            webClient.Headers.Add(RequestHeaderConstants.AuthAccept, F);
            webClient.Headers.Add(HttpRequestHeader.ContentType, DataAccessLayerConstants.ContentTypeJson);
            webClient.Headers.Add(HttpRequestHeader.Accept, DataAccessLayerConstants.ContentTypeJson);
            return webClient;
        }

        /// <summary>
        ///     Creates a new ReferenceListItem, adds header to request and writes the request
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="uri"></param>
        public override void AddListReferenceItem(string listName, Uri uri)
        {
            try
            {
                var listItem = CreateNewReferenceListItem(listName, uri);
                var wreq = CreateRequest(-1, listName);
                wreq.ContentLength = listItem.Length;

                var writer = new StreamWriter(wreq.GetRequestStream());
                writer.Write(listItem);
                writer.Flush();
                wreq.GetResponse();
            }
            catch (Exception exception)
            {
                Exception currentException =
                    new RestOperationException(DefaultExceptionMessages.RestAddExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        /// <summary>
        ///     Creates a httpwebrequest
        ///     If itemId is -1 then it's obvious it's a POST request and acts as needed
        ///     Elsewhere it's a MERGE request
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest(int itemId, string listName)
        {
            if (itemId == -1)
            {
                var httpWebRequest = BuildCommonRequest(string.Format(ApiConstants.ListItemsApi, listName));
                BuildHttpRequestHeader(httpWebRequest, RequestHeaderConstants.Post);
                return httpWebRequest;
            }
            else
            {
                var httpWebRequest = BuildCommonRequest(string.Format(ApiConstants.ListItemByIdApi, listName, itemId));
                BuildHttpRequestHeader(httpWebRequest, RequestHeaderConstants.Merge);
                return httpWebRequest;
            }
        }

        private HttpWebRequest BuildCommonRequest(string apiResult)
        {
            var connectionUri = new Uri(Path.Combine(ConnectionConfiguration.Connection.Uri.AbsoluteUri,
                apiResult));
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(connectionUri);
            httpWebRequest.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName,
                ConnectionConfiguration.Connection.Credentials.Password);
            return httpWebRequest;
        }

        /// <summary>
        ///     Changes a ListReferenceItem identified by it's <paramref name="itemId" />
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="itemId"></param>
        /// <param name="listName"></param>
        public override void ChangeListReferenceItem(Uri uri, int itemId, string listName)
        {
            try
            {
                var listItem = CreateNewReferenceListItem(listName, uri);
                var wreq = CreateRequest(itemId, listName);
                wreq.ContentLength = listItem.Length;

                var writer = new StreamWriter(wreq.GetRequestStream());
                writer.Write(listItem);
                writer.Flush();
                wreq.GetResponse();
            }
            catch (Exception exception)
            {
                Exception currentException =
                    new RestOperationException(DefaultExceptionMessages.RestChangeExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        /// <summary>
        ///     Removes a ListReferenceItem identified by it's <paramref name="itemId" />
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="itemId"></param>
        public override void RemoveListReferenceItem(string listName, int itemId)
        {
            try
            {
                var webClient = BuildWebClientWithHeader();
                webClient.Headers.Add(RequestHeaderConstants.Digest, RequestFormDigest());
                webClient.Headers.Add(HttpRequestHeader.IfMatch, StarAll);
                webClient.Headers.Add(RequestHeaderConstants.Method, RequestHeaderConstants.Delete);
                webClient.Headers.Add(HttpRequestHeader.ContentType, DataAccessLayerConstants.ContentTypeJson);
                var url = ConnectionConfiguration.Connection.Uri +
                          string.Format(ApiConstants.ListItemByIdApi, listName, itemId);
                var endpointUri = new Uri(url);
                webClient.UploadString(endpointUri, RequestHeaderConstants.Post);
            }
            catch (Exception exception)
            {
                Exception currentException =
                    new RestOperationException(DefaultExceptionMessages.RestRemoveExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        private IEnumerable<XElement> GetSearchedXElements(Stream receiveStream)
        {
            using (var stream = receiveStream)
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        var result = sr.ReadToEnd().Trim();
                        var elements = XElement.Parse(result);
                        var wantedElements = from primaryQuery in elements.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.PQR)
                                             from relevantResults in primaryQuery.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.RelevantResults)
                                             from table in relevantResults.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.Table)
                                             from rows in table.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.Rows)
                                             from elementss in rows.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.Element)
                                             from cells in elementss.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.Cells)
                                             select cells;
                        return wantedElements;
                    }
                }
                return new List<XElement>();
            }
        }

        private Dictionary<string, string> CreateDictionaryFromXelements(Stream receiveStream)
        {
            var wantedElements = GetSearchedXElements(receiveStream);
            var elementsPath = wantedElements.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.Element).
            Where(x => x.Element(DataAccessLayerConstants.DNamespace + SearchConstants.Key).
            Value.StartsWith(SearchConstants.ElementPath))
            .Select(x => x.Value.Remove(SearchConstants.RemoveStartIndex, SearchConstants.ElementPath.Length).Replace(SearchConstants.UselessTitleSegment, ""))
            .ToList();
            var elementsTitle = wantedElements.Elements(DataAccessLayerConstants.DNamespace + SearchConstants.Element)
                .Where(x => x.Element(DataAccessLayerConstants.DNamespace + SearchConstants.Key).
            Value.Equals(SearchConstants.Title))
            .Select(x => x.Value.Remove(SearchConstants.RemoveStartIndex, SearchConstants.Title.Length).Replace(SearchConstants.UselessTitleSegment, ""))
            .ToList();
            var elementsDictionary = elementsPath.Zip(elementsTitle, (path, title) => new { path, title })
            .ToDictionary(x => x.path, x => x.title);
            return elementsDictionary;
        }



        public override Dictionary<string, string> SearchSPFiles(string wantedItem)
        {
            var request = BuildCommonRequest(string.Format(ApiConstants.SearchItems, wantedItem));
            var response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            Dictionary<string, string> searchedElementsOnWantedSiteCollection = new Dictionary<string, string>();
            var elementsDictionary = CreateDictionaryFromXelements(receiveStream);
            foreach (var element in elementsDictionary)
            {
                if (element.Key.StartsWith(ConnectionConfiguration.Connection.UriString))
                {
                    foreach (var list in ConnectionConfiguration.ListsWithColumnsNames)
                    {
                        if (!element.Key.Contains(list.ListName))
                        {
                            searchedElementsOnWantedSiteCollection.Add(element.Key, element.Value);
                        }
                    }
                }
            }
            return searchedElementsOnWantedSiteCollection;
        }
    }
}

