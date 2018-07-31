namespace BusinessLogicLayer
{
    using System;
    using Common.ApplicationEnums;
    using Configuration;

    /// <summary>
    ///     Manages ListReference with an instance of BaseListReferenceProvider
    /// </summary>
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
        public void AddSyncListItem(string url, string listName)
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            listReferenceProvider.AddListReferenceItem(listName, new Uri(url));
        }

        /// <summary>
        ///     Calls RemoveListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void RemoveSyncListItem(int id, string listName)
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

        public void SearchFiles()
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            listReferenceProvider.SearchSPFiles();
        }

    }
}