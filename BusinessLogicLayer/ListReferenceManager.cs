namespace BusinessLogicLayer
{
    using System;
    using Configuration;
    using DataAccessLayer;
    using Common.ApplicationEnums;


    /// <summary>
    /// Manages ListReference with an instance of BaseListReferenceProvider
    /// </summary>
    public class ListReferenceManager
    {
        //TODO [CR RT] : Use capital letters for properties connectionConfiguration - > ConnectionConfiguration
        private ConnectionConfiguration connectionConfiguration { get; }

        private ListReferenceProviderType providerType { get; }

        public ListReferenceManager(ConnectionConfiguration configuration, ListReferenceProviderType type)
        {
            connectionConfiguration = configuration;
            providerType = type;
        }

        /// <summary>
        /// Calls AddListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="url"></param>
        /// <param name="listName"></param>
        public void AddSyncListItem(string url, string listName)
        {
            BaseListReferenceProvider listReferenceProvider = OperationsFactory.GetOperations(providerType);
            listReferenceProvider.ConnectionConfiguration = connectionConfiguration;
            listReferenceProvider.AddListReferenceItem(listName, new Uri(url));
        }

        /// <summary>
        /// Calls RemoveListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void RemoveSyncListItem(int id, string listName)
        {
            BaseListReferenceProvider listReferenceProvider = OperationsFactory.GetOperations(providerType);
            listReferenceProvider.ConnectionConfiguration = connectionConfiguration;
            listReferenceProvider.RemoveListReferenceItem(listName, id);
        }

        /// <summary>
        /// Calls ChangeListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void ChangeSyncListItem(string url, int id, string listName)
        {
            BaseListReferenceProvider listReferenceProvider = OperationsFactory.GetOperations(providerType);
            listReferenceProvider.ConnectionConfiguration = connectionConfiguration;
            listReferenceProvider.ChangeListReferenceItem(new Uri(url), id, listName);
        }
    }
}
