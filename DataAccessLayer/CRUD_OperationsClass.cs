using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public abstract class CRUD_OperationsClass
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public abstract void AddListReferenceItem(string listName, Uri uri);

        public abstract void RemoveListReferenceItem(string listName, int itemID);

        public abstract void ChangeListReferenceItem(Uri uri, int itemID, string listName);

        public string CreateNewReferenceListItem(string listName, Uri uri, string userName)
        {
            string url = uri.AbsoluteUri;
            var listWithColumn = ConnectionConfiguration.ListsWithColumnsNames.First(list => list.ListName == listName);
            return "{'__metadata': { 'type': 'SP.Data." + listName + "ListItem' }" +

                ", '" + listWithColumn.UrlColumnName + "': " +
                "{ '__metadata': { 'type': 'SP.FieldUrlValue' }, " +
                "'Url': '" + url + "','Description': '" + url + "'}" +
                ", '" + listWithColumn.UserColumnName + "Id' : "
                + ConnectionConfiguration.Connection.GetCurrentUserId() + "}";
        }

        public DateTime GetMetadataItem(string url)
        {
            return MetadataGetter.GetModifiedDateOfItem(ConnectionConfiguration, url);
        }

        public List<string> GetCurrentUserUrls()
        {
            return MetadataGetter.GetCurrentUserUrls(ConnectionConfiguration);
        }
    }
}
