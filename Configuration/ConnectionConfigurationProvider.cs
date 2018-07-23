namespace Configuration
{
    using System;
    using Models;

    /// <summary>
    ///     Provides or modifies a ConnectionConfiguration based on given parameters
    /// </summary>
    public class ConnectionConfigurationProvider
    {
        /// <summary>
        ///     Creates a new ConnectionConfiguration which has a new Connection(based on parameters) and default value for the
        ///     rest of properties
        /// </summary>
        /// <param name="spUrl"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ConnectionConfiguration CreateConnectionConfiguration(string spUrl, string user, string password)
        {
            var newConnection = new Connection
            {
                Uri = new Uri(spUrl),
                Credentials = new Credentials {UserName = user, Password = password}
            };
            return new ConnectionConfiguration {Connection = newConnection};
        }
    }
}