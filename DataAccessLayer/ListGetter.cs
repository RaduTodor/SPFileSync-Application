using Configuration;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using SP = Microsoft.SharePoint.Client;

namespace DataAccessLayer
{
    public static class ListGetter
    {
        public static SP.List GetList(ConnectionConfiguration connectionConfiguration, string url)
        {
            ClientContext context = connectionConfiguration.Connection.SharePointResult();
            Web site = context.Web;
            context.Load(site, osite => osite.Title);
            context.ExecuteQuery();
            return site.GetList(url);
        }

        public static IEnumerable<SP.List> GetLists(ConnectionConfiguration connectionConfiguration)
        {
            ClientContext context = connectionConfiguration.Connection.SharePointResult();
            Web site = context.Web;
            context.Load(site, osite => osite.Title);
            context.ExecuteQuery();
            ListCollection lists = site.Lists;
            IEnumerable<SP.List> listsCollection =
                context.LoadQuery(lists.Include(l => l.Title, l => l.Id));
            context.ExecuteQuery();
            return listsCollection;
        }

        public static IEnumerable<SP.ListItem> GetAllListItems(ConnectionConfiguration connectionConfiguration, string ListName)
        {
            using (var ctx = connectionConfiguration.Connection.SharePointResult())
            {
                var qry = new CamlQuery();
                qry.ViewXml = "<View Scope='RecursiveAll'>" +
                                         "<Query>" +
                                             "<Where>" +
                                                   "<Eq>" +
                                                        "<FieldRef Name='FSObjType' />" +
                                                        "<Value Type='Integer'>0</Value>" +
                                                   "</Eq>" +
                                            "</Where>" +
                                          "</Query>" +
                                       "</View>";

                var sourceList = ctx.Web.Lists.GetByTitle(ListName);
                var items = sourceList.GetItems(qry);
                ctx.Load(items);
                ctx.ExecuteQuery();
                return items;
            }
        }
    }
}
