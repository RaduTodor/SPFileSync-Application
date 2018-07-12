﻿using Configuration;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using SP = Microsoft.SharePoint.Client;

namespace DataAccessLayer
{
    public class CRUD_Operations
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public string ListName { get; set; }

        public string GetCurrentUserName()
        {
            return ConnectionConfiguration.Connection.Credentials.UserName.Split('\\')[1].Split('\\')[0];
        }

        public SP.User GetCurrentUser()
        {
            return ConnectionConfiguration.Connection.SharePointResult().Web.CurrentUser;
        }

        public void AddListItem()
        {
            using (var client = new SharepointList(ConnectionConfiguration.Connection.Uri, ConnectionConfiguration.Connection.Credentials))
            {
                var contactEntry = new
                {
                    metadata = new { type = $"SP.Data.{ListName}ListItem" },
                    Title = "testtt"
                };
                client.PostListItem(ListName, contactEntry);
            }
        }

        public void RemoveListItem(int itemID)
        {
            using (var client = new SharepointList(ConnectionConfiguration.Connection.Uri, ConnectionConfiguration.Connection.Credentials))
            {
                client.DeleteListItem(ListName, itemID);
            }
        }

        public string GetMetadataItem(string url)
        {
            using (var client = new SharepointList(ConnectionConfiguration.Connection.Uri, ConnectionConfiguration.Connection.Credentials))
            {
                string json=client.GetMetadataFileItem(url);
                
                return json;
            }
        }

        public void ChangeListItem(int itemID)
        {
            using (var client = new SharepointList(ConnectionConfiguration.Connection.Uri, ConnectionConfiguration.Connection.Credentials))
            {
                var contactEntry = new
                {
                    metadata = new { type = $"SP.Data.{ListName}ListItem" },
                    Title = "testtt"
                };
                client.PutListItem(ListName, contactEntry, itemID);
            }
        }

        public void AddListReferenceItem(System.Uri uri)
        {
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            SP.List oList = clientContext.Web.Lists.GetByTitle(ListName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);
            oListItem["URL"] = uri;
            oListItem["User"] = clientContext.Web.CurrentUser;
            oListItem.Update();
            clientContext.ExecuteQuery();
        }

        public void RemoveListReferenceItem(int itemID)
        {
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            SP.List oList = clientContext.Web.Lists.GetByTitle(ListName);
            oList.GetItemById(itemID).DeleteObject();
            clientContext.ExecuteQuery();
        }

        public void ChangeListReferenceItem(System.Uri uri, int itemID)
        {
            var clientContext = ConnectionConfiguration.Connection.SharePointResult();
            SP.List oList = clientContext.Web.Lists.GetByTitle(ListName);
            ListItem oListItem = oList.GetItemById(itemID);
            oListItem["URL"] = uri;
            oListItem["User"] = clientContext.Web.CurrentUser;
            oListItem.Update();
            clientContext.ExecuteQuery();
        }

        public SP.List GetList(string url)
        {
            ClientContext context = ConnectionConfiguration.Connection.SharePointResult();
            Web site = context.Web;
            context.Load(site, osite => osite.Title);
            context.ExecuteQuery();
            return site.GetList(url);
        }

        public IEnumerable<SP.List> GetLists()
        {
            ClientContext context = ConnectionConfiguration.Connection.SharePointResult();
            Web site = context.Web;
            context.Load(site, osite => osite.Title);
            context.ExecuteQuery();
            ListCollection lists = site.Lists;
            IEnumerable<SP.List> listsCollection =
                context.LoadQuery(lists.Include(l => l.Title, l => l.Id));
            context.ExecuteQuery();
            return listsCollection;
        }

        public IEnumerable<SP.ListItem> GetListItems()
        {
            using (var ctx = ConnectionConfiguration.Connection.SharePointResult())
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
