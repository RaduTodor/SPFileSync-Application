using Configuration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Net;
using SP = Microsoft.SharePoint.Client;

namespace DataAccessLayer
{
    public class CRUD_Operations
    {
        public string ListName { get; set; }

        public void AddListItem(Uri uri, ICredentials credentials)
        {
            using (var client = new SharepointList(uri, credentials))
            {
                var contactEntry = new
                {
                    metadata = new { type = $"SP.Data.{ListName}ListItem" },
                    Title = "testtt"
                };
                client.PostListItem(ListName, contactEntry);
            }
        }

        public void RemoveListItem(Uri uri, ICredentials credentials, int itemID)
        {
            using (var client = new SharepointList(uri, credentials))
            {
                client.DeleteListItem(ListName, itemID);
            }
        }

        public void ChangeListItem(Uri uri, ICredentials credentials, int itemID)
        {
            using (var client = new SharepointList(uri, credentials))
            {
                var contactEntry = new
                {
                    metadata = new { type = $"SP.Data.{ListName}ListItem" },
                    Title = "testtt"
                };
                client.PutListItem(ListName, contactEntry, itemID);
            }
        }

        public IEnumerable<SP.List> GetLists()
        {
            ClientContext context = Connection.SharePointResult();
            Web site = context.Web;
            context.Load(site, osite => osite.Title);
            context.ExecuteQuery();
            ListCollection lists = site.Lists;
            IEnumerable<SP.List> listsCollection =
                context.LoadQuery(lists.Include(l => l.Title, l => l.Id));
            context.ExecuteQuery();
            return listsCollection;
        }

        public IEnumerable<SP.ListItem> GetListItems(string listName)
        {
            using (var ctx = Connection.SharePointResult())
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

                var sourceList = ctx.Web.Lists.GetByTitle(listName);
                var items = sourceList.GetItems(qry);
                ctx.Load(items);
                ctx.ExecuteQuery();
                return items;
            }
        }
    }
}
