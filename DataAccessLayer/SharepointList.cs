using Configuration;
using Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

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

        public DateTime GetModifiedDateOfItem(string fileUrl)
        {
            fileUrl = fileUrl.Replace("%20", " ");
            string listTitle = ParseURLParentDirectory(fileUrl);
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(WebUri.ToString() + 
                $"_api/Web/lists/getbytitle('{listTitle}')/items?" +
                $"$select=Modified" +
                $"&$filter=FileRef eq '{fileUrl}'");
            endpointRequest.Method = "GET";
            endpointRequest.Credentials = webClient.Credentials;
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

        public List<string> GetCurrentUserUrls(ConnectionConfiguration connectionConfiguration)
        {
            List<string> allUrlsOfCurrentUser = new List<string>();
            foreach (ListWithColumnsName listWithColumnsName in connectionConfiguration.ListsWithColumnsNames)
            {
                HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(WebUri.ToString() + 
                    $"_api/Web/lists/getbytitle('{listWithColumnsName.ListName}')/items?" +
                    $"$select={listWithColumnsName.UrlColumnName}" +
                    $"&$filter={listWithColumnsName.UserColumnName} eq '{connectionConfiguration.Connection.GetCurrentUserName()}'");
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
                        allUrlsOfCurrentUser.AddRange(GetAllUrlsInResponse(result, listWithColumnsName.UrlColumnName));
                    }
                }
            }
            return allUrlsOfCurrentUser;
        }
        private readonly XNamespace metadataBaseNamespace = "http://www.w3.org/2005/Atom";
        private readonly XNamespace mNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private readonly XNamespace dNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        public List<string> GetAllUrlsInResponse(string xmlString, string urlColumnName)
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

        public DateTime GetModifiedDateInResponse(string xmlString)
        {
            var elements = XElement.Parse(xmlString);
            var result = from entryBody in elements.Elements(metadataBaseNamespace + "entry")
                         from contentBody in entryBody.Elements(metadataBaseNamespace + "content")
                         from propertiesBody in contentBody.Elements(mNamespace + "properties")
                         from modifiedDate in propertiesBody.Elements(dNamespace + "Modified")
                         select modifiedDate;
            return Convert.ToDateTime(result.First().Value);
        }

        public string ParseURLParentDirectory(string url)
        {
            Uri uri = new Uri(url);
            uri = new Uri(uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length));
            string final = uri.Segments[3];
            final = final.Remove(final.Length - 1);
            return final.Replace("%20", " ");
        }
        #region CommentedCode
        //public void PostListItem(string listTitle, object payload)
        //{
        //    var formDigestValue = RequestFormDigest("POST");
        //    webClient.Headers.Add("X-RequestDigest", formDigestValue);
        //    var url = WebUri + string.Format("_api/web/lists/getbytitle('{0}')/items", listTitle);
        //    var endpointUri = new Uri(url);
        //    var payloadString = JsonConvert.SerializeObject(payload);
        //    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
        //    webClient.UploadString(endpointUri, "POST", payloadString);
        //}

        //public void DeleteListItem(string listTitle, int id)
        //{
        //    var formDigestValue = RequestFormDigest("DELETE");
        //    webClient.Headers.Add("X-RequestDigest", formDigestValue);
        //    var url = WebUri + string.Format("_api/web/lists/getbytitle('{0}')/items/getbyid({1})", listTitle, id);
        //    var endpointUri = new Uri(url);
        //    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
        //    webClient.UploadString(endpointUri, "DELETE");
        //}

        //public void PutListItem(string listTitle, object payload, int id)
        //{
        //    var formDigestValue = RequestFormDigest("PUT");
        //    webClient.Headers.Add("X-RequestDigest", formDigestValue);
        //    var url = WebUri + string.Format("_api/web/lists/getbytitle('{0}')/items/getbyid({1})", listTitle, id);
        //    var endpointUri = new Uri(url);
        //    var payloadString = JsonConvert.SerializeObject(payload);
        //    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
        //    webClient.UploadString(endpointUri, "PUT", payloadString);
        //}
        #endregion
        public void Dispose()
        {
            webClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
