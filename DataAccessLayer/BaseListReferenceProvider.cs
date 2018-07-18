namespace DataAccessLayer
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Constants;

    //TODO [CR RT]: Add class and methods documentation

    public abstract class BaseListReferenceProvider
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public abstract void AddListReferenceItem(string listName, Uri uri);

        public abstract void RemoveListReferenceItem(string listName, int itemId);

        public abstract void ChangeListReferenceItem(Uri uri, int itemId, string listName);

        protected string CreateNewReferenceListItem(string listName, Uri uri)
        {
            var url = uri.AbsoluteUri;
            var listWithColumn = ConnectionConfiguration.ListsWithColumnsNames.First(list => list.ListName == listName);
            return string.Format(QuerryTemplates.NewReferenceItem, listName, listWithColumn.UrlColumnName, url,
                listWithColumn.UserColumnName, ConnectionConfiguration.Connection.GetCurrentUserId());
        }

        public DateTime GetMetadataItem(string url)
        {
            var metadataProvider = new MetadataProvider(ConnectionConfiguration);
            return metadataProvider.GetModifiedDateOfItem(url);
        }

        public List<string> GetCurrentUserUrls()
        {
            var metadataProvider = new MetadataProvider(ConnectionConfiguration);
            return metadataProvider.GetCurrentUserUrls();
        }
    }
}
