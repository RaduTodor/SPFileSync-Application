namespace Configuration
{
    using System;

    public class ConnectionConfigurationProvider
    {
        public static ConnectionConfiguration CreateConnectionConfiguration(string spUrl, string user, string password)
        {
            var newConnection = new Connection
            {
                Uri = new Uri(spUrl),
                Credentials = new Models.Credentials { UserName = user, Password = password }
            };
            return new ConnectionConfiguration { Connection = newConnection };
        }
    }
}
