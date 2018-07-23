namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using Common.Constants;
    using Common.Exceptions;
    using Common.Helpers;
    using Configuration;
    using Microsoft.SharePoint.Client;

    /// <summary>
    ///     Has methods which get sharepoint Lists or ListItems.
    ///     Needs to be instanced
    /// </summary>
    public class ListProvider
    {
        public ListProvider(ConnectionConfiguration configuration)
        {
            ConnectionConfiguration = configuration;
        }

        private ConnectionConfiguration ConnectionConfiguration { get; }

        /// <summary>
        ///     Gets the List from a sharepoint site given by <paramref name="url" />
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List GetList(string url)
        {
            try
            {
                using (var context = ConnectionConfiguration.Connection.CreateContext())
                {
                    var site = context.Web;
                    context.Load(site, currentWeb => currentWeb.Title);
                    context.ExecuteQuery();
                    return site.GetList(url);
                }
            }
            catch (Exception exception)
            {
                Exception currentException =
                    new GetRequestException(DefaultExceptionMessages.GetRequestExceptionMessage, exception);
                MyLogger.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        /// <summary>
        ///     Gets all Lists from the sharepoint site devined in configuration
        /// </summary>
        public IEnumerable<List> GetLists()
        {
            try
            {
                using (var context = ConnectionConfiguration.Connection.CreateContext())
                {
                    var site = context.Web;
                    context.Load(site, currentWeb => currentWeb.Title);
                    context.ExecuteQuery();
                    var lists = site.Lists;
                    var listsCollection =
                        context.LoadQuery(
                            lists.Include(currentList => currentList.Title, currentList => currentList.Id));
                    context.ExecuteQuery();
                    return listsCollection;
                }
            }
            catch (Exception exception)
            {
                Exception currentException =
                    new GetRequestException(DefaultExceptionMessages.GetRequestExceptionMessage, exception);
                MyLogger.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }

        /// <summary>
        ///     Gets all ListItems from a List given the <paramref name="listName" />
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetAllListItems(string listName)
        {
            try
            {
                using (var clientContext = ConnectionConfiguration.Connection.CreateContext())
                {
                    var query = new CamlQuery
                    {
                        ViewXml = QuerryTemplates.AllListItems
                    };

                    var sourceList = clientContext.Web.Lists.GetByTitle(listName);
                    var items = sourceList.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();
                    return items;
                }
            }
            catch (Exception exception)
            {
                Exception currentException =
                    new GetRequestException(DefaultExceptionMessages.GetRequestExceptionMessage, exception);
                MyLogger.Logger.Error(currentException, currentException.Message);
                throw currentException;
            }
        }
    }
}