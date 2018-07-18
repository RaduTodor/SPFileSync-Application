using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    //TODO [CR RT]: Rename class. Use intuitive naming e.g. BaseListReferenceProvider

    //TODO [CR RT]: Add class and methods documentation

    public abstract class CRUD_OperationsClass
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public abstract void AddListReferenceItem(string listName, Uri uri);

        //TODO [CR RT]: use camel case naming itemID -> itemId
        public abstract void RemoveListReferenceItem(string listName, int itemID);

        //TODO [CR RT]: use camel case naming itemID -> itemId
        public abstract void ChangeListReferenceItem(Uri uri, int itemID, string listName);

        //TODO [CR RT]: Extract constants in Common DLL, use string.Format
        //TODO [CR RT]: Remove unused parameter
        //TODO [CR RT]: Make method protected

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
