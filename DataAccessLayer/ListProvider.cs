namespace DataAccessLayer
{
    using Configuration;
    using Microsoft.SharePoint.Client;
    using System.Collections.Generic;
    using Common.Constants;

    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Exception handling on all public methods

    public class ListProvider
    {
        public ListProvider(ConnectionConfiguration configuration)
        {
            ConnectionConfiguration = configuration;
        }
        private ConnectionConfiguration ConnectionConfiguration { get; }

        public List GetList(string url)
        {
            using (var context = ConnectionConfiguration.Connection.CreateContext())
            {
                var site = context.Web;
                context.Load(site, currentWeb => currentWeb.Title);
                context.ExecuteQuery();
                return site.GetList(url);
            }
        }

        public IEnumerable<List> GetLists()
        {
            using (var context = ConnectionConfiguration.Connection.CreateContext())
            {
                var site = context.Web;
                context.Load(site, currentWeb => currentWeb.Title);
                context.ExecuteQuery();
                var lists = site.Lists;
                var listsCollection =
                    context.LoadQuery(lists.Include(currentList => currentList.Title, currentList => currentList.Id));
                context.ExecuteQuery();
                return listsCollection;
            }
        }

        public IEnumerable<ListItem> GetAllListItems(string listName)
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
    }
}
