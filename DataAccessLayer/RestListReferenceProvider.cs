namespace DataAccessLayer
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Net;
    using Common.Constants;

    //TODO [CR RT]: Add class and methods documentation
    public class RestListReferenceProvider : BaseListReferenceProvider
    {
        private string RequestFormDigest()
        {
            var webClient = BuildWebClientWithHeader();
            var url = ConnectionConfiguration.Connection.Uri + "_api/contextinfo";
            var endpointUri = new Uri(url);
            var result = webClient.UploadString(endpointUri, "POST");
            var token = JToken.Parse(result);
            if (token != null)
            {
                return token["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private void BuildHttpRequestHeader(HttpWebRequest request, string operationType)
        {
            request.Method = "POST";
            request.Accept = "application/json; odata=verbose";
            request.ContentType = "application/json; odata=verbose";
            request.Headers.Add("X-HTTP-Method", operationType);
            if (operationType == "MERGE")
                request.Headers.Add("IF-MATCH", "*");
            request.Headers.Add("X-RequestDigest", RequestFormDigest());
        }

        public WebClient BuildWebClientWithHeader()
        {
            var webClient = new WebClient
            {
                Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName,
                    ConnectionConfiguration.Connection.Credentials.Password)
            };
            webClient.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            return webClient;
        }


        public override void AddListReferenceItem(string listName, Uri uri)
        {
            var listItem = CreateNewReferenceListItem(listName, uri);
            var wreq = CreateRequest(uri, -1, listName);
            wreq.ContentLength = listItem.Length;

            var writer = new StreamWriter(wreq.GetRequestStream());
            writer.Write(listItem);
            writer.Flush();
        }

        private HttpWebRequest CreateRequest(Uri uri, int itemId, string listName)
        {
            if (itemId == -1)
            {
                var connectionUri = new Uri(Path.Combine(ConnectionConfiguration.Connection.Uri.AbsoluteUri, 
                                        string.Format(QuerryTemplates.ListItemsApi, listName)));
                var wreq = (HttpWebRequest)WebRequest.Create(connectionUri);
                wreq.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName, ConnectionConfiguration.Connection.Credentials.Password);
                BuildHttpRequestHeader(wreq, "POST");
                return wreq;
            }
            else
            {
                var connectionUri = new Uri(Path.Combine(ConnectionConfiguration.Connection.Uri.AbsoluteUri,
                                        string.Format(QuerryTemplates.ListItemByIdApi, listName, itemId)));
                var wreq = (HttpWebRequest)WebRequest.Create(connectionUri);
                wreq.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName, ConnectionConfiguration.Connection.Credentials.Password);
                BuildHttpRequestHeader(wreq, "MERGE");
                return wreq;
            }
        }

        public override void ChangeListReferenceItem(Uri uri, int itemId, string listName)
        {
            var listItem = CreateNewReferenceListItem(listName, uri);
            var wreq = CreateRequest(uri, itemId, listName);
            wreq.ContentLength = listItem.Length;
            
            var writer = new StreamWriter(wreq.GetRequestStream());
            writer.Write(listItem);
            writer.Flush();
        }

        //TODO [CR RT]: Extract header build in a separate method
        public override void RemoveListReferenceItem(string listName, int itemId)
        {
            var webClient = BuildWebClientWithHeader();
            webClient.Headers.Add("X-RequestDigest", RequestFormDigest());
            webClient.Headers.Add(HttpRequestHeader.IfMatch, "*");
            webClient.Headers.Add("X-HTTP-Method", "DELETE");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            var url = ConnectionConfiguration.Connection.Uri + string.Format(QuerryTemplates.ListItemsApi, listName, itemId);
            var endpointUri = new Uri(url);
            webClient.UploadString(endpointUri, "POST");
        }
    }
}
