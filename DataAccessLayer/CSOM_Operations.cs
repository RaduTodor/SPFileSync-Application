using Configuration;
using Microsoft.SharePoint.Client;
using Models;
using System;
using System.Linq;

namespace DataAccessLayer
{
    //TODO [CR RT]: Rename class. Use intuitive naming e.g. CsomListReferenceProvider
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Remove unused using
    //TODO [CR RT]: Handle duplicate code from AddListReferenceItem and ChangeListReferenceItem methods. Extract duplicate code in a new method. Keep in existing methods just the particular logic.
    //TODO [CR RT]: Error handling in all public methods -> in general

    public class CSOM_Operations : CRUD_OperationsClass
    {
        //TODO [CR RT]: Give intuitive naming for bigList, oListItem, oList, listWithColumns etc.

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

        //TODO [CR RT]: use camel case naming itemID -> itemId
        //TODO [CR RT]: Give intuitive naming for bigList, oListItem, oList, listWithColumns etc.
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

        //TODO [CR RT]: use camel case naming itemID -> itemId
        //TODO [CR RT]: Give intuitive naming for  oList etc.
        public override void RemoveListReferenceItem(string listName, int itemID)
        {
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            List oList = clientContext.Web.Lists.GetByTitle(listName);
            oList.GetItemById(itemID).DeleteObject();
            clientContext.ExecuteQuery();
        }
    }
}
