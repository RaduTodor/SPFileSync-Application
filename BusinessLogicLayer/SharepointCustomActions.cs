namespace BusinessLogicLayer
{
    using Configuration;
    using Microsoft.SharePoint.Client;

    public class SharepointCustomActions
    {
        public SharepointCustomActions(ConnectionConfiguration config)
        {
            Configuration = config;
        }

        private ConnectionConfiguration Configuration { get; }

        public void DeleteUserCustomAction(string libraryName, string actionName, string location)
        {
            using (var clientContext = Configuration.Connection.CreateContext())
            {
                var customlist = LoadListFromTitle(libraryName, clientContext);
                clientContext.Load(customlist,
                    list => list.UserCustomActions);
                clientContext.ExecuteQuery();
                var collUserCustomAction =
                    customlist.UserCustomActions;
                var customAction = CustomActionExists(collUserCustomAction, actionName, location);
                if (customAction != null)
                {
                    customAction.DeleteObject();
                    customlist.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }

        public void AddNewCustomAction(string libraryName, string actionName, string location)
        {
            using (var clientContext = Configuration.Connection.CreateContext())
            {
                var customlist = LoadListFromTitle(libraryName, clientContext);
                var collUserCustomAction =
                    customlist.UserCustomActions;
                if (CustomActionExists(collUserCustomAction, actionName, location) == null)
                {

                    AddNewCustomAction(collUserCustomAction, actionName, location, CustomActionUrl, 100);
                    clientContext.Load(customlist,
                        list => list.UserCustomActions);
                    clientContext.ExecuteQuery();
                }
            }
        }

        private void AddNewCustomAction(UserCustomActionCollection collUserCustomAction, string title,
            string location, string url, int sequence)
        {
            var newcustomaAction = collUserCustomAction.Add();
            newcustomaAction.Location = location;
            newcustomaAction.Sequence = sequence;
            newcustomaAction.Title = title;
            newcustomaAction.Url = CustomActionUrl;
            newcustomaAction.Update();
        }

        private UserCustomActionCollection GetUserCustomActionCollection(string actionName, string location,
            List customlist)
        {
            var collUserCustomAction =
                customlist.UserCustomActions;
            return collUserCustomAction;
        }

        private UserCustomAction CustomActionExists(UserCustomActionCollection collUserCustomAction, string actionName,
            string location)
        {
            foreach (var verifyCustomAction in collUserCustomAction)
                if (verifyCustomAction.Title.Equals(actionName) &&
                    verifyCustomAction.Location.Equals(location))
                    return verifyCustomAction;
            return null;
        }

        private List LoadListFromTitle(string libraryName, ClientContext clientContext)
        {
            var customlist = clientContext.Web.Lists.GetByTitle(libraryName);
            clientContext.Load(customlist);
            clientContext.ExecuteQuery();
            return customlist;
        }

        private const string CustomActionUrl = @"javascript:
    (function(e,s){e.src=s;e.onload=function(){jQuery.noConflict();console.log('jQuery injected')};document.head.appendChild(e);})(document.createElement('script'),'//code.jquery.com/jquery-latest.min.js')
    setTimeout(function () {
    var selectedListItemId = SP.ListOperation.Selection.getSelectedItems()[0].id;
    var currentUrl = _spPageContextInfo.siteServerRelativeUrl;
    var currentListId = SP.ListOperation.Selection.getSelectedList();
    jQuery.ajax({
        url: 'http://sp2013dc' + currentUrl + '/_api/web/lists(\''+ currentListId + '\')/items(' + selectedListItemId + ')?$select=FileRef',
        method: 'GET',
        headers: { 'Accept': 'application/json; odata=verbose' },
        success: function (data) {
            var itemReturnedUrl = data.d.FileRef;
            var item = {
                '__metadata': {
                    'type': 'SP.Data.SyncListListItem'
                },
                'URL': {
                    '__metadata': {
                        'type': SP.FieldUrlValue
                    },
                    'Url': itemReturnedUrl,
                    'Description': itemReturnedUrl
                },
                'UserId': _spPageContextInfo.userId
            };
            jQuery.ajax({
                url: 'http://sp2013dc' + currentUrl + '/_api/web/lists(\'{9AFEF388-B887-4B3A-AD69-DC37DFD7F540}\')/items',
                type: 'POST',
                contentType: 'application/json;odata=verbose',
                data: JSON.stringify(item),
                headers: {
                    'Accept': 'application/json;odata=verbose',
                    'X-RequestDigest': jQuery('#__REQUESTDIGEST').val(),
                    'X-HTTP-Method': 'POST'
                },
                error: function (data) {
                    console.log('There was a problem while Adding the selected file to the refeence list!');
                }
            });
        },
        error: function (data) {
            console.log('There was a problem while getting the selected item url!');
        }
    });}, 500);";
    }
}