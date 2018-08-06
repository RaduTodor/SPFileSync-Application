namespace BusinessLogicLayer
{
    using System;
    using Common.ApplicationEnums;
    using Configuration;

    /// <summary>
    ///     Manages ListReference with an instance of BaseListReferenceProvider
    /// </summary>
    /// 
    ///TODO[CR BT]: Extract code ducplication. Each method has two lines that are the same. Extract them in a method and call it on constructor if it's the case.
    public class ListReferenceManager
    {
        public ListReferenceManager(ConnectionConfiguration configuration, ListReferenceProviderType type)
        {
            ConnectionConfiguration = configuration;
            ProviderType = type;
        }

        private ConnectionConfiguration ConnectionConfiguration { get; }

        private ListReferenceProviderType ProviderType { get; }

        /// <summary>
        ///     Calls AddListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="url"></param>
        /// <param name="listName"></param>
        public void AddSyncListItem(string listName, string url)
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            listReferenceProvider.AddListReferenceItem(listName, new Uri(url));
        }

        public void AddItemToSyncList(string url)
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
        }

        /// <summary>
        ///     Calls RemoveListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void RemoveSyncListItem(string listName, int id)
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            listReferenceProvider.RemoveListReferenceItem(listName, id);
        }

        /// <summary>
        ///     Calls ChangeListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void ChangeSyncListItem(string url, int id, string listName)
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            listReferenceProvider.ChangeListReferenceItem(new Uri(url), id, listName);
        }

        public void SearchFiles(string item)
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            listReferenceProvider.SearchSPFiles(item);
        }

    }
}