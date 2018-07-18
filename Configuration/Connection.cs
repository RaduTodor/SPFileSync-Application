namespace Configuration
{
    using System;
    using System.Net;
    using Microsoft.SharePoint.Client;
    using System.Linq;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using Models;

    //TODO [CR RT]: Add class and methods documentation
    public class Connection
    {
        private const char Backslash = '\\';

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

        public ClientContext CreateContext()
        {
            var context = new ClientContext(Uri);
            context.Credentials = new NetworkCredential(Credentials.UserName,Credentials.Password);
            return context;
        }

        public string GetCurrentUserName()
        {
            return Credentials != null ? Credentials.UserName.Split(Backslash)[1].Split(Backslash)[0] : "";
        }

        public string GetSharepointIdentifier()
        {
            var result = Uri.Segments.Last();
            return result.Substring(0, result.Length - 1);
        }

        public int GetCurrentUserId()
        {
            using (var clientContext = CreateContext())
            {
                if (clientContext != null)
                {
                    var oWebsite = clientContext.Web;

                    clientContext.Load(oWebsite,
                        w => w.CurrentUser);
                }

                if (clientContext != null)
                {
                    try
                    {
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception exception)
                    {

                    }
                    return clientContext.Web.CurrentUser.Id;
                }
            }

            return -1;
        }
    }
}
