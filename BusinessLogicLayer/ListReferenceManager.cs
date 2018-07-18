namespace BusinessLogicLayer
{
    using System;
    using Common.Constants;
    using Configuration;
    using DataAccessLayer;

    public class ListReferenceManager
    {
        private ConnectionConfiguration connectionConfiguration { get; }

        private ApplicationEnums.ListReferenceProviderType providerType { get; }

        public ListReferenceManager(ConnectionConfiguration configuration, ApplicationEnums.ListReferenceProviderType type)
        {
            connectionConfiguration = configuration;
            providerType = type;
        }

        public void AddSyncListItem(string url, string listName)
        {
            BaseListReferenceProvider listReferenceProvider = OperationsFactory.GetOperations(providerType);
            listReferenceProvider.ConnectionConfiguration = connectionConfiguration;
            listReferenceProvider.AddListReferenceItem(listName, new Uri(url));
        }

        public void RemoveSyncListItem(int id, string listName)
        {
            BaseListReferenceProvider listReferenceProvider = OperationsFactory.GetOperations(providerType);
            listReferenceProvider.ConnectionConfiguration = connectionConfiguration;
            listReferenceProvider.RemoveListReferenceItem(listName, id);
        }

        public void ChangeSyncListItem(string url, int id, string listName)
        {
            BaseListReferenceProvider listReferenceProvider = OperationsFactory.GetOperations(providerType);
            listReferenceProvider.ConnectionConfiguration = connectionConfiguration;
            listReferenceProvider.ChangeListReferenceItem(new Uri(url), id, listName);
        }
    }
}
