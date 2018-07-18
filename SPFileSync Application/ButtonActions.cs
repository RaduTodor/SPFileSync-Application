using BusinessLogicLayer;
using Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPFileSync_Application
{
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Remove static from methods. Initialize connectionConfiguration,restOrCsom from ctor, make it private
    //TODO [CR RT]: Move class to BusinessLogicLayer DLL
    //TODO [CR RT]: Rename class e.g. FilesManager.
    public class ButtonActions
    {
        //TODO [CR RT]: Rename method to Synchronize
        public static void SynchronizeButtonPressed(List<ConnectionConfiguration> connections, int restOrCsom)
        {
            foreach (var connection in connections)
            {
                FileSynchronizer fileSync = new FileSynchronizer { DataAccessOperations = new DataAccessOperations(connection, restOrCsom) };
                Task t = Task.Run(() => fileSync.Synchronize());
            }
        }

        //TODO [CR RT]: Extract to different class in Connection DLL
        public static ConnectionConfiguration AddConnectionButtonPressed(string spUrl, string user, string password, int restOrCsom)
        {
            Connection newConnection = new Connection
            {
                Uri = new Uri(spUrl),
                Credentials = new Models.Credentials { UserName = user, Password = password }
            };
            return new ConnectionConfiguration { Connection = newConnection };
        }


        //TODO [CR RT]: Extract methods from below to BLL DLL in a new class.
        //TODO [CR RT]: Give apropriate naming e.g. ListReferenceManager. This should replace the logic of Operations from DataAccessOperations
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
