namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Constants;
    using Configuration;

    /// <summary>
    ///     It's the base of a ListReferenceProvider
    /// </summary>
    public abstract class BaseListReferenceProvider
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public abstract void AddListReferenceItem(string listName, Uri uri);

        public abstract void RemoveListReferenceItem(string listName, int itemId);

        public abstract void SearchSPFiles();

        public abstract void ChangeListReferenceItem(Uri uri, int itemId, string listName);

        /// <summary>
        ///     Creates the querry needed for a new ReferenceListItem to be made
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string CreateNewReferenceListItem(string listName, Uri uri)
        {
            var url = uri.AbsoluteUri;
            var listWithColumn = ConnectionConfiguration.ListsWithColumnsNames.First(list => list.ListName == listName);
            return string.Format(ApiConstants.NewReferenceItem, listName, listWithColumn.UrlColumnName, url, url,
                listWithColumn.UserColumnName, ConnectionConfiguration.Connection.GetCurrentUserId());
        }

        /// <summary>
        ///     Returns the ModifiedDate of an listItem based on it's url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="internetAccessException"></param>
        /// <returns></returns>
        public DateTime GetMetadataItem(string url, EventHandler<Exception> internetAccessException)
        {
            var metadataProvider = new MetadataProvider(ConnectionConfiguration);
            return metadataProvider.GetModifiedDateOfItem(url, internetAccessException);
        }

        /// <summary>
        ///     Gets all urls from all ReferenceListItem of CurrentUser
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentUserUrls(EventHandler<Exception> exceptionHandler, EventHandler<Exception> internetAccessException)
        {
            var metadataProvider = new MetadataProvider(ConnectionConfiguration);
            return metadataProvider.GetCurrentUserUrls(exceptionHandler,internetAccessException);
        }
    }
}