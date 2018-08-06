namespace BusinessLogicLayer
{
    using System;
    using Common.ApplicationEnums;
    using Configuration;

    /// <summary>
    ///     Manages ListReference with an instance of BaseListReferenceProvider
    /// </summary>
    /// 
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
            GetListReferenceProvider().AddListReferenceItem(listName, new Uri(url));
        }

        public void AddItemToSyncList(string url)
        {
            GetListReferenceProvider();
        }

        /// <summary>
        ///     Calls RemoveListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void RemoveSyncListItem(string listName, int id)
        {
            GetListReferenceProvider().RemoveListReferenceItem(listName, id);
        }

        /// <summary>
        ///     Calls ChangeListReferenceItem of current instance of BaseListReferenceProvider
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="listName"></param>
        public void ChangeSyncListItem(string url, int id, string listName)
        {
            GetListReferenceProvider().ChangeListReferenceItem(new Uri(url), id, listName);
        }

        private DataAccessLayer.BaseListReferenceProvider GetListReferenceProvider()
        {
            var listReferenceProvider = OperationsFactory.GetOperations(ProviderType);
            listReferenceProvider.ConnectionConfiguration = ConnectionConfiguration;
            return listReferenceProvider;
        }

        public void SearchFiles(string item)
        {
            GetListReferenceProvider().SearchSPFiles(item);
        }

    }
}