namespace DataAccessLayer
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Net;
    using Common.Constants;

    /// <summary>
    /// An implementation of BaseListReferenceProvider which uses REST calls technology to implement base methods.
    /// </summary>
    public class RestListReferenceProvider : BaseListReferenceProvider
    {
        private const string StarAll = "*";

        private const string F = "f";

        /// <summary>
        /// Gets the Digest Value needed for authentication
        /// </summary>
        /// <returns></returns>
        private string RequestFormDigest()
        {
            var webClient = BuildWebClientWithHeader();
            var url = ConnectionConfiguration.Connection.Uri + ApiConstants.ContextInfo;
            var endpointUri = new Uri(url);
            var result = webClient.UploadString(endpointUri, RequestHeaderConstants.Post);
            JToken token = JToken.Parse(result);
            if (token != null)
            {
                return token[DataAccessLayerConstants.JTokenFirstLayer][DataAccessLayerConstants.JTokenSecondLayer][DataAccessLayerConstants.JTokenThirdLayer].ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Build HttpRequestHeader needed for  Post and Merge operation
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
        /// Build WebClientHeader needed for Remove operation and fost GetDigestValue method
        /// </summary>
        /// <returns></returns>
        private WebClient BuildWebClientWithHeader()
        {
            var webClient = new WebClient
            {
                Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName,
                    ConnectionConfiguration.Connection.Credentials.Password)
            };
            webClient.Headers.Add(RequestHeaderConstants.AuthAccept, F);
            webClient.Headers.Add(HttpRequestHeader.ContentType, DataAccessLayerConstants.ContentTypeJson);
            webClient.Headers.Add(HttpRequestHeader.Accept, DataAccessLayerConstants.ContentTypeJson);
            return webClient;
        }

        /// <summary>
        /// Creates a new ReferenceListItem, adds header to request and writes the request
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
            catch (Exception e)
            {

            }

        }

        /// <summary>
        /// Creates a httpwebrequest
        /// If itemId is -1 then it's obvious it's a POST request and acts as needed
        /// Elsewhere it's a MERGE request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="itemId"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest(int itemId, string listName)
        {
            if (itemId == -1)
            {
                var httpWebRequest = WebRequestCommonBuilder(string.Format(ApiConstants.ListItemsApi, listName));
                BuildHttpRequestHeader(httpWebRequest, RequestHeaderConstants.Post);
                return httpWebRequest;
            }
            else
            {
                var httpWebRequest = WebRequestCommonBuilder(string.Format(ApiConstants.ListItemByIdApi, listName, itemId));
                BuildHttpRequestHeader(httpWebRequest, RequestHeaderConstants.Merge);
                return httpWebRequest;
            }
        }

        private HttpWebRequest WebRequestCommonBuilder(string apiResult)
        {
            var connectionUri = new Uri(Path.Combine(ConnectionConfiguration.Connection.Uri.AbsoluteUri,
                apiResult));
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(connectionUri);
            httpWebRequest.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName,
                ConnectionConfiguration.Connection.Credentials.Password);
            return httpWebRequest;
        }

        /// <summary>
        /// Changes a ListReferenceItem identified by it's <paramref name="itemId"/>
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="itemId"></param>
        /// <param name="listName"></param>
        public override void ChangeListReferenceItem(Uri uri,int itemId, string listName)
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
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Removes a ListReferenceItem identified by it's <paramref name="itemId"/>
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
                var url = ConnectionConfiguration.Connection.Uri + string.Format(ApiConstants.ListItemByIdApi, listName, itemId);
                var endpointUri = new Uri(url);
                webClient.UploadString(endpointUri, RequestHeaderConstants.Post);
            }
            catch (Exception e)
            {

            }
        }
    }
}
