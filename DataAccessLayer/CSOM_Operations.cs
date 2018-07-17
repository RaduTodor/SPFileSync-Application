using Configuration;
using Microsoft.SharePoint.Client;
using Models;
using System;
using System.Linq;

namespace DataAccessLayer
{
    public class CSOM_Operations : CRUD_OperationsClass
    {
        public override void AddListReferenceItem(string listName, Uri uri)
        {
            ListWithColumnsName listWithColumns = ConnectionConfiguration.ListsWithColumnsNames.First(bigList => bigList.ListName == listName);
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            List oList = clientContext.Web.Lists.GetByTitle(listName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);
            oListItem[$"{listWithColumns.UrlColumnName}"] = uri;
            oListItem[$"{listWithColumns.UserColumnName}"] = clientContext.Web.CurrentUser;
            oListItem.Update();
            clientContext.ExecuteQuery();
        }

        public override void ChangeListReferenceItem(Uri uri, int itemID, string listName)
        {
            ListWithColumnsName listWithColumns = ConnectionConfiguration.ListsWithColumnsNames.First(bigList => bigList.ListName == listName);
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            List oList = clientContext.Web.Lists.GetByTitle(listWithColumns.ListName);
            ListItem oListItem = oList.GetItemById(itemID);
            oListItem[$"{listWithColumns.UrlColumnName}"] = uri;
            oListItem[$"{listWithColumns.UserColumnName}"] = clientContext.Web.CurrentUser;
            oListItem.Update();
            clientContext.ExecuteQuery();
        }

        public override void RemoveListReferenceItem(string listName, int itemID)
        {
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            List oList = clientContext.Web.Lists.GetByTitle(listName);
            oList.GetItemById(itemID).DeleteObject();
            clientContext.ExecuteQuery();
        }
    }
}
