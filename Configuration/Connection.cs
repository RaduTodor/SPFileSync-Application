namespace Configuration
{
    using System;
    using System.Net;
    using Microsoft.SharePoint.Client;
    using System.Linq;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using Models;

    /// <summary>
    /// Connection instance has an Uri and Credentials needed for a connection to be made
    /// It also has some useful methods
    /// </summary>
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
        
        /// <summary>
        /// Returns a ClientContext for instance's Uri with instance's Credentials
        /// </summary>
        /// <returns></returns>
        public ClientContext CreateContext()
        {
            var context = new ClientContext(Uri);
            context.Credentials = new NetworkCredential(Credentials.UserName,Credentials.Password);
            return context;
        }

        /// <summary>
        /// Gets the username part without domain (just the login name)
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserName()
        {
            return Credentials != null ? Credentials.UserName.Split(Backslash)[1].Split(Backslash)[0] : "";
        }

        /// <summary>
        /// Returns last part of a sharepoint uri
        /// </summary>
        /// <returns></returns>
        public string GetSharepointIdentifier()
        {
            var result = Uri.Segments.Last();
            return result.Substring(0, result.Length - 1);
        }

        /// <summary>
        /// Loads a clientContext with instance's data and returns the current User Id's
        /// </summary>
        /// <returns></returns>
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
