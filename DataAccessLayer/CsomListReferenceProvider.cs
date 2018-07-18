namespace DataAccessLayer
{
    using Microsoft.SharePoint.Client;
    using Models;
    using System;
    using System.Linq;
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Error handling in all public methods -> in general

    public class CsomListReferenceProvider : BaseListReferenceProvider
    {
        public override void AddListReferenceItem(string listName, Uri uri)
        {
            var list = ConnectionConfiguration.ListsWithColumnsNames.First(configurationList => configurationList.ListName == listName);
            var clientContext = ConnectionConfiguration.Connection.CreateContext();
            var referenceList = clientContext.Web.Lists.GetByTitle(listName);
            var itemCreateInfo = new ListItemCreationInformation();
            var listItem = referenceList.AddItem(itemCreateInfo);
            UpdateListReferenceItem(listItem,list,uri,clientContext);
        }

        public override void ChangeListReferenceItem(Uri uri, int itemId, string listName)
        {
            var list = ConnectionConfiguration.ListsWithColumnsNames.First(configurationList => configurationList.ListName == listName);
            var clientContext = ConnectionConfiguration.Connection.CreateContext();
            var referenceList = clientContext.Web.Lists.GetByTitle(list.ListName);
            var listItem = referenceList.GetItemById(itemId);
            UpdateListReferenceItem(listItem, list, uri, clientContext);
        }

        private void UpdateListReferenceItem(ListItem listItem, ListWithColumnsName list, Uri uri, ClientContext clientContext)
        {
            listItem[$"{list.UrlColumnName}"] = uri;
            listItem[$"{list.UserColumnName}"] = clientContext.Web.CurrentUser;
            listItem.Update();
            clientContext.ExecuteQuery();
        }

        public override void RemoveListReferenceItem(string listName, int itemId)
        {
            var clientContext = ConnectionConfiguration.Connection.CreateContext();
            var referenceList = clientContext.Web.Lists.GetByTitle(listName);
            referenceList.GetItemById(itemId).DeleteObject();
            clientContext.ExecuteQuery();
        }
    }
}
