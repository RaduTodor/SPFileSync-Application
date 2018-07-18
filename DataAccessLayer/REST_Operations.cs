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
    //TODO [CR RT]: Rename class. Use intuitive naming e.g. RestListReferenceProvider
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Remove unused using
    //TODO [CR RT]: Extract contant values to Common DLL if are used in multiple places, otherwise extract them on class level (private const string fields)
    //TODO [CR RT]: All Api Urls to be extracted to a constant class from Common DLL 
    public class REST_Operations : CRUD_OperationsClass
    {
        //TODO [CR RT]: Give intuitive naming for t etc.
        private string RequestFormDigest()
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName, ConnectionConfiguration.Connection.Credentials.Password);
            webClient.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            var url = ConnectionConfiguration.Connection.Uri + "_api/contextinfo";
            var endpointUri = new Uri(url);
            var result = webClient.UploadString(endpointUri, "POST");
            JToken t = JToken.Parse(result);
            //TODO [CR RT]: Check for null
            return t["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
        }

        //TODO [CR RT]: Remove unnecessary code, lines for result member
        //TODO [CR RT]: Extract header build in a separate method
        //TODO [CR RT]: Extract request and conection creation in a separate method
        public override void AddListReferenceItem(string listName, Uri uri)
        {
            string listItem = CreateNewReferenceListItem(listName, uri, ConnectionConfiguration.Connection.GetCurrentUserName());
            string result = string.Empty;
            //TODO [CR RT]: USe Path.Combine or String.Format
            Uri connectionUri = new Uri(ConnectionConfiguration.Connection.Uri + string.Format($"_api/web/lists/getbytitle('{listName}')/items"));
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(connectionUri);
            wreq.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName, ConnectionConfiguration.Connection.Credentials.Password);

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

        //TODO [CR RT]: Remove unnecessary code, lines for result member
        //TODO [CR RT]: Rename itemID ->itemId
        //TODO [CR RT]: Apply the same rules from the above method

        public override void ChangeListReferenceItem(Uri uri, int itemID, string listName)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName, ConnectionConfiguration.Connection.Credentials.Password);
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

        //TODO [CR RT]: Extract header build in a separate method
        public override void RemoveListReferenceItem(string listName, int itemID)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(ConnectionConfiguration.Connection.Credentials.UserName, ConnectionConfiguration.Connection.Credentials.Password);
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
