using System;
using System.Net;
using System.Configuration;
using Microsoft.SharePoint.Client;

namespace Configuration
{
    public class Connection
    {
        public Connection(Uri uri, NetworkCredential credentials)
        {
            Uri = uri;
            Credentials = credentials;
        }

        public static Uri Uri = new Uri(ConfigurationManager.AppSettings["SharePointURL"]);

        public static NetworkCredential Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Account"], ConfigurationManager.AppSettings["Password"]);

        public static ClientContext SharePointResult()
        {
            ClientContext context = new ClientContext(Uri);
            context.Credentials = Credentials;
            return context;
        }
    }
}
