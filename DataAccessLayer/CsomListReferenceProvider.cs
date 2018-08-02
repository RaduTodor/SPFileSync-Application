namespace DataAccessLayer
{
    using System;
    using System.Linq;
    using Common.Constants;
    using Common.Exceptions;
    using Common.Helpers;
    using Microsoft.SharePoint.Client;
    using Models;
    using Microsoft.SharePoint.Client.Search.Query;
    using System.Collections.Generic;

    /// <summary>
    ///     An implementation of BaseListReferenceProvider which uses CSOM technology to implement base methods.
    /// </summary>
    public class CsomListReferenceProvider : BaseListReferenceProvider
    {
        /// <summary>
        ///     Calls UdateListReferenceItem for an added item created with AddItem
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
                Exception currentException =
                    new CsomOperationException(DefaultExceptionMessages.CsomAddExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        /// <summary>
        ///     Calls UdateListReferenceItem for an existing item found with GetByTitle
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
                Exception currentException =
                    new CsomOperationException(DefaultExceptionMessages.CsomChangeExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        /// <summary>
        ///     Changes properties of given ReferenceListItem and uploads it on sharepoint ClientContext
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="list"></param>
        /// <param name="uri"></param>
        /// <param name="clientContext"></param>
        private void UpdateListReferenceItem(ListItem listItem, ListWithColumnsName list, Uri uri,
            ClientContext clientContext)
        {
            listItem[$"{list.UrlColumnName}"] = uri;
            listItem[$"{list.UserColumnName}"] = clientContext.Web.CurrentUser;
            listItem.Update();
            clientContext.ExecuteQuery();
        }

        private void NewConfigSelected(List<string> listsName, List<ListWithColumnsName> lists)
        {
            listsName = new List<string>();
            lists = ConnectionConfiguration.ListsWithColumnsNames;
            listsName = new List<string>();
            foreach (var item in lists) listsName.Add(item.ListName);
        }

        public override Dictionary<string, string> SearchSPFiles(string item)
        {
            ClientContext clientContext = ConnectionConfiguration.Connection.CreateContext();            
            KeywordQuery keywordQuery = new KeywordQuery(clientContext);
            Dictionary<string, string> wantedItems = new Dictionary<string, string>();
            keywordQuery.QueryText = item;                
            keywordQuery.RefinementFilters.Add("FileType:or" + $"('{Extensions.CSV}','{Extensions.PDF}','{Extensions.TXT}','{Extensions.XLS}','{Extensions.XML}','{Extensions.DOCX}','{Extensions.XLSX}')");              
            SearchExecutor searchExecutor = new SearchExecutor(clientContext);
            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(keywordQuery);
            clientContext.ExecuteQuery();
            var result = results.Value;
            foreach (var resultRow in results.Value[0].ResultRows)
            {
                if (resultRow[HelpersConstants.Path].ToString().StartsWith(ConnectionConfiguration.Connection.UriString))
                {
                    foreach (var list in ConnectionConfiguration.ListsWithColumnsNames)
                    {
                        if (!resultRow[HelpersConstants.Path].ToString().Contains(list.ListName))
                        {
                            wantedItems.Add(resultRow[HelpersConstants.Path].ToString(), resultRow[SearchConstants.Title].ToString());
                        }
                    }
                }
            }
            return wantedItems;
        }
   
        /// <summary>
        ///     Removes ReferenceListItem found with GetByTitle, with DeleteObject
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
                Exception currentException =
                    new CsomOperationException(DefaultExceptionMessages.CsomRemoveExceptionMessage, exception);
                LoggerManager.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }
    }
}