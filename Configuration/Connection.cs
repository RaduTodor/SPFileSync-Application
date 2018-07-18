using System;
using System.Net;
using Microsoft.SharePoint.Client;
using System.Linq;

namespace Configuration
{
    //TODO [CR RT]: Add class and methods documentation
    public class Connection
    {
        public Uri Uri { get; set; }

        public NetworkCredential Credentials { get; set; }

        //TODO [CR RT]: Rename method e.g. CreateContext
        public ClientContext SharePointResult()
        {
            ClientContext context = new ClientContext(Uri);
            context.Credentials = Credentials;
            return context;
        }

        public string GetCurrentUserName()
        {
            //TODO [CR RT]: Exttract constants
            //TODO [CR RT]: Check for null or empty before additinal operation on the item
            return Credentials.UserName.Split('\\')[1].Split('\\')[0];
        }

        public string GetSharepointIdentifier()
        {
            string result = Uri.Segments.Last();
            return result.Substring(0, result.Length - 1);
        }

        //TODO [CR RT]: Check for null and exception handling on clientContext
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
