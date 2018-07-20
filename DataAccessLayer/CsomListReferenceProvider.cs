using Common.Constants;
using Common.Exceptions;

namespace DataAccessLayer
{
    using Microsoft.SharePoint.Client;
    using Models;
    using System;
    using System.Linq;

    /// <summary>
    /// An implementation of BaseListReferenceProvider which uses CSOM technology to implement base methods.
    /// </summary>
    public class CsomListReferenceProvider : BaseListReferenceProvider
    {
        /// <summary>
        /// Calls UdateListReferenceItem for an added item created with AddItem
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="uri"></param>
        public override void AddListReferenceItem(string listName, Uri uri)
        {
            try
            {
                var list = ConnectionConfiguration.ListsWithColumnsNames.First(configurationList =>
                    configurationList.ListName == listName);
                var clientContext = ConnectionConfiguration.Connection.CreateContext();
                var referenceList = clientContext.Web.Lists.GetByTitle(listName);
                var itemCreateInfo = new ListItemCreationInformation();
                var listItem = referenceList.AddItem(itemCreateInfo);
                UpdateListReferenceItem(listItem, list, uri, clientContext);
            }
            catch (Exception exception)
            {
                throw new CsomOperationException(DefaultExceptionMessages.CsomAddExceptionMessage,exception);
            }
        }

        /// <summary>
        /// Calls UdateListReferenceItem for an existing item found with GetByTitle
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="itemId"></param>
        /// <param name="listName"></param>
        public override void ChangeListReferenceItem(Uri uri, int itemId, string listName)
        {
            try
            {
                var list = ConnectionConfiguration.ListsWithColumnsNames.First(configurationList =>
                    configurationList.ListName == listName);
                var clientContext = ConnectionConfiguration.Connection.CreateContext();
                var referenceList = clientContext.Web.Lists.GetByTitle(list.ListName);
                var listItem = referenceList.GetItemById(itemId);
                UpdateListReferenceItem(listItem, list, uri, clientContext);
            }
            catch (Exception exception)
            {
                throw new CsomOperationException(DefaultExceptionMessages.CsomChangeExceptionMessage, exception);
            }
        }

        /// <summary>
        /// Changes properties of given ReferenceListItem and uploads it on sharepoint ClientContext 
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="list"></param>
        /// <param name="uri"></param>
        /// <param name="clientContext"></param>
        private void UpdateListReferenceItem(ListItem listItem, ListWithColumnsName list, Uri uri, ClientContext clientContext)
        {
            listItem[$"{list.UrlColumnName}"] = uri;
            listItem[$"{list.UserColumnName}"] = clientContext.Web.CurrentUser;
            listItem.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Removes ReferenceListItem found with GetByTitle, with DeleteObject 
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="itemId"></param>
        public override void RemoveListReferenceItem(string listName, int itemId)
        {
            try
            {
                var clientContext = ConnectionConfiguration.Connection.CreateContext();
                var referenceList = clientContext.Web.Lists.GetByTitle(listName);
                referenceList.GetItemById(itemId).DeleteObject();
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                throw new CsomOperationException(DefaultExceptionMessages.CsomRemoveExceptionMessage, exception);
            }
        }
    }
}
