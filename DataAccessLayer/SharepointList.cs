using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Linq;
using System.Text;
using Configuration;
using System.Collections.Generic;
using Models;
using Microsoft.SharePoint.Client;
using System.Xml.Linq;
using System.Xml;

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

        public string GetMetadataFileItem(string fileUrl)
        {
            fileUrl = fileUrl.Replace("%20", " ");
            string listTitle = ParseURLParentDirectory(fileUrl);
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(WebUri.ToString() + $"_api/Web/lists/getbytitle('{listTitle}')/items?$select=Modified&$filter=FileRef eq '{fileUrl}'");
            endpointRequest.Method = "GET";
            endpointRequest.Credentials = webClient.Credentials;
            endpointRequest.Accept = "application/json;odata=verbose";
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            string result = "";
            using (System.IO.Stream stream = endpointResponse.GetResponseStream())
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return result;
        }

        public void GetCurrentUserItems(ConnectionConfiguration connectionConfiguration)
        {
            foreach (ListWithColumnsName listWithColumnsName in connectionConfiguration.ListsWithColumnsNames)
            {
                int id = 0;
                using (ClientContext context = connectionConfiguration.Connection.SharePointResult()) {
                    context.Load(context.Web);
                    context.Load(context.Web.CurrentUser);
                    context.ExecuteQuery();
                    id = context.Web.CurrentUser.Id;
                }
                HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(WebUri.ToString() + $"_api/Web/lists/getbytitle('{listWithColumnsName.ListName}')" +
                    $"/items?" +
                    $"&$filter=User eq {id}");
                endpointRequest.Method = "GET";
                endpointRequest.Credentials = webClient.Credentials;
                endpointRequest.Accept = "application/xml;odata=verbose";
                HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
                string result = "";
                using (System.IO.Stream stream = endpointResponse.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                    {
                        result = sr.ReadToEnd().Trim();
                        XDocument doc = XDocument.Parse(result);
                        var elements = XElement.Parse(result);
                        elements.
                        sr.Close();
                    }
                }

            }
        }

        public string ParseURLParentDirectory(string url)
        {
            Uri uri = new Uri(url);
            uri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            string final = uri.Segments[3];
            final = final.Remove(final.Length - 1);
            final = final.Replace("%20", " ");
            return final;
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
