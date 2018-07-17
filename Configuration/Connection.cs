using System;
using System.Net;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using Models;

namespace Configuration
{
    public class Connection
    {
        [XmlIgnore]
        public Uri Uri { get; set; }

        [XmlAttribute("uri")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string UriString
        {
            get { return Uri == null ? null : Uri.ToString(); }
            set { Uri = value == null ? null : new Uri(value); }
        }

        public Credentials Credentials { get; set; }

        public ClientContext SharePointResult()
        {
            ClientContext context = new ClientContext(Uri);
            context.Credentials = new NetworkCredential(Credentials.UserName,Credentials.Password);
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
