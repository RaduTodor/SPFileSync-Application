using BusinessLogicLayer;
using Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPFileSync_Application
{
    public class ButtonActions
    {
        public static void SynchronizeButtonPressed(List<ConnectionConfiguration> connections, int restOrCsom)
        {
            foreach (var connection in connections)
            {
                FileSynchronizer fileSync = new FileSynchronizer { DataAccessOperations = new DataAccessOperations(connection, restOrCsom) };
                Task t = Task.Run(() => fileSync.Synchronize());
            }
        }

        public static ConnectionConfiguration AddConnectionButtonPressed(string spUrl, string user, string password, int restOrCsom)
        {
            Connection newConnection = new Connection
            {
                Uri = new Uri(spUrl),
                Credentials = new Models.Credentials { UserName = user, Password = password }
            };
            return new ConnectionConfiguration { Connection = newConnection };
        }

        public static void AddSyncListItem(ConnectionConfiguration connection, string url, string listName, int restOrCsom)
        {
            DataAccessOperations dataAccessOperations = new DataAccessOperations(connection, restOrCsom);
            dataAccessOperations.Operations.AddListReferenceItem(listName, new Uri(url));
        }

        public static void RemoveSyncListItem(ConnectionConfiguration connection, int id, string listName, int restOrCsom)
        {
            DataAccessOperations dataAccessOperations = new DataAccessOperations(connection, restOrCsom);
            dataAccessOperations.Operations.RemoveListReferenceItem(listName, id);
        }

        public static void ChangeSyncListItem(ConnectionConfiguration connection, string url, int id, string listName, int restOrCsom)
        {
            DataAccessOperations dataAccessOperations = new DataAccessOperations(connection, restOrCsom);
            dataAccessOperations.Operations.ChangeListReferenceItem(new Uri(url), id, listName);
        }
    }
}
