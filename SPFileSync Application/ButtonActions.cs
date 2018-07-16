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

        public static void AddSyncListItem(ConnectionConfiguration connection, string url, string listName)
        {
            DataAccessOperations dataAccessOperations = new DataAccessOperations(connection);
            dataAccessOperations.Operations.AddListReferenceItem(new Uri(url), listName);
        }

        public static void RemoveSyncListItem(ConnectionConfiguration connection, int id, string listName)
        {
            DataAccessOperations dataAccessOperations = new DataAccessOperations(connection);
            dataAccessOperations.Operations.RemoveListReferenceItem(id, listName);
        }

        public static void ChangeSyncListItem(ConnectionConfiguration connection, string url, int id, string listName)
        {
            DataAccessOperations dataAccessOperations = new DataAccessOperations(connection);
            dataAccessOperations.Operations.ChangeListReferenceItem(new Uri(url), id , listName);
        }
    }
}
