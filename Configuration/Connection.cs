using System;
using System.Net;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace Configuration
{
    public class Connection
    {
        public Uri Uri { get; set; }

        public NetworkCredential Credentials { get; set; }

        public ClientContext SharePointResult()
        {
            ClientContext context = new ClientContext(Uri);
            context.Credentials = Credentials;
            return context;
        }

        public string GetCurrentUserName()
        {
            return Credentials.UserName.Split('\\')[1].Split('\\')[0];
        }

        public string GetSharepointIdentifier()
        {
            string result = Uri.Segments.Last();
            return result.Substring(0, result.Length - 1);
        }

        public int GetCurrentUserId()
        {
            ClientContext clientContext = SharePointResult();
            Web oWebsite = clientContext.Web;

            clientContext.Load(oWebsite,
                w => w.CurrentUser);

            clientContext.ExecuteQuery();


            return clientContext.Web.CurrentUser.Id;
        }
    }
}
