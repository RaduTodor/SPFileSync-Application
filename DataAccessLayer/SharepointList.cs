using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace DataAccessLayer
{
    public class SharepointList : IDisposable
    {
        private readonly WebClient webClient;

        public Uri WebUri { get; private set; }

        public SharepointList(Uri webUri, ICredentials credentials)
        {
            webClient = new WebClient();
            webClient.Credentials = credentials;
            webClient.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            WebUri = webUri;
        }

        private string RequestFormDigest(string Operation)
        {
            var url = WebUri + "_api/contextinfo";
            var endpointUri = new Uri(url);
            var result = webClient.UploadString(endpointUri, Operation);
            JToken t = JToken.Parse(result);
            return t["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
        }

        public void PostListItem(string listTitle, object payload)
        {
            var formDigestValue = RequestFormDigest("POST");
            webClient.Headers.Add("X-RequestDigest", formDigestValue);
            var url = WebUri + string.Format("_api/web/lists/getbytitle('{0}')/items", listTitle);
            var endpointUri = new Uri(url);
            var payloadString = JsonConvert.SerializeObject(payload);
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.UploadString(endpointUri, "POST", payloadString);
        }

        public void DeleteListItem(string listTitle, int id)
        {
            var formDigestValue = RequestFormDigest("DELETE");
            webClient.Headers.Add("X-RequestDigest", formDigestValue);
            var url = WebUri + string.Format("_api/web/lists/getbytitle('{0}')/items({1})", listTitle, id);
            var endpointUri = new Uri(url);
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.UploadString(endpointUri, "DELETE");
        }

        public void PutListItem(string listTitle, object payload, int id)
        {
            var formDigestValue = RequestFormDigest("PUT");
            webClient.Headers.Add("X-RequestDigest", formDigestValue);
            var url = WebUri + string.Format("_api/web/lists/getbytitle('{0}')/items({1})", listTitle, id);
            var endpointUri = new Uri(url);
            var payloadString = JsonConvert.SerializeObject(payload);
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.UploadString(endpointUri, "PUT", payloadString);
        }

        public void Dispose()
        {
            webClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
