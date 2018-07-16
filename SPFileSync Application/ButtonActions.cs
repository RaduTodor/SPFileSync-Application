using BusinessLogicLayer;
using Configuration;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPFileSync_Application
{
    public class ButtonActions
    {
        public static void SynchronizeButtonPressed(List<ConnectionConfiguration> connections)
        {
            foreach(var connection in connections)
            {
                FileSynchronizer fileSync = new FileSynchronizer { DataAccessOperations = new DataAccessOperations(connection) };
                Task t = Task.Run(() => fileSync.Synchronize());
            }
        }

        public static ConnectionConfiguration AddConnectionButtonPressed(string spUrl, string user, string password)
        {
            Connection newConnection = new Connection { Uri = new Uri(spUrl),
                Credentials = new System.Net.NetworkCredential(user, password) };
            return new ConnectionConfiguration { Connection = newConnection };
        }
    }
}
