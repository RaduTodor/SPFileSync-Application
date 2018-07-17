using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class REST_Operations : CRUD_OperationsClass
    {
        private string RequestFormDigest()
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = ConnectionConfiguration.Connection.Credentials;
            webClient.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            var url = ConnectionConfiguration.Connection.Uri + "_api/contextinfo";
            var endpointUri = new Uri(url);
            var result = webClient.UploadString(endpointUri, "POST");
            JToken t = JToken.Parse(result);
            return t["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
        }

        public override void AddListReferenceItem(string listName, Uri uri)
        {
            string listItem = CreateNewReferenceListItem(listName, uri, ConnectionConfiguration.Connection.GetCurrentUserName());
            string result = string.Empty;
            Uri connectionUri = new Uri(ConnectionConfiguration.Connection.Uri + string.Format($"_api/web/lists/getbytitle('{listName}')/items"));
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(connectionUri);
            wreq.Credentials = ConnectionConfiguration.Connection.Credentials;

            wreq.Method = "POST";
            wreq.Accept = "application/json; odata=verbose";
            wreq.ContentType = "application/json; odata=verbose";
            wreq.Headers.Add("X-HTTP-Method", "POST");
            wreq.Headers.Add("X-RequestDigest", RequestFormDigest());

            wreq.ContentLength = listItem.Length;
            StreamWriter writer = new StreamWriter(wreq.GetRequestStream());
            writer.Write(listItem);
            writer.Flush();

            WebResponse wresp = wreq.GetResponse();
            using (StreamReader sr = new StreamReader(wresp.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }
        }

        public override void ChangeListReferenceItem(Uri uri, int itemID, string listName)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = ConnectionConfiguration.Connection.Credentials;
            webClient.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            string listItem = CreateNewReferenceListItem(listName, uri, ConnectionConfiguration.Connection.GetCurrentUserName());
            string result = string.Empty;
            Uri connectionUri = new Uri(ConnectionConfiguration.Connection.Uri + string.Format($"_api/web/lists/getbytitle('{listName}')/items({itemID})"));
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(connectionUri);
            wreq.Credentials = webClient.Credentials;

            wreq.Method = "POST";
            wreq.Accept = "application/json; odata=verbose";
            wreq.ContentType = "application/json; odata=verbose";
            wreq.Headers.Add("X-HTTP-Method", "MERGE");
            wreq.Headers.Add("IF-MATCH", "*");
            wreq.Headers.Add("X-RequestDigest", RequestFormDigest());

            wreq.ContentLength = listItem.Length;
            StreamWriter writer = new StreamWriter(wreq.GetRequestStream());
            writer.Write(listItem);
            writer.Flush();

            WebResponse wresp = wreq.GetResponse();
            using (StreamReader sr = new StreamReader(wresp.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }
        }

        public override void RemoveListReferenceItem(string listName, int itemID)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = ConnectionConfiguration.Connection.Credentials;
            webClient.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            var formDigestValue = RequestFormDigest();
            webClient.Headers.Add("X-RequestDigest", formDigestValue);
            webClient.Headers.Add(HttpRequestHeader.IfMatch, "*");
            webClient.Headers.Add("X-HTTP-Method", "DELETE");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            var url = ConnectionConfiguration.Connection.Uri + string.Format($"_api/web/lists/getbytitle('{listName}')/items({itemID})");
            var endpointUri = new Uri(url);
            webClient.UploadString(endpointUri, "POST");
        }
    }
}
