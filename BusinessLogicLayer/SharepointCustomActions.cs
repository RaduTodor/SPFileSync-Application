namespace BusinessLogicLayer
{
    using Configuration;
    using Microsoft.SharePoint.Client;
    //TODO[CR BT]: Remove unused usings
    //TODO[CR BT]: Why this class is not used
    public class SharepointCustomActions
    {
        public SharepointCustomActions(ConnectionConfiguration config)
        {
            Configuration = config;
        }

        private ConnectionConfiguration Configuration { get; }

        //TODO[CR BT]: Extract in multiple methods
        public void DeleteUserCustomAction(string libraryName, string actionName, string location)
        {
            using (var clientContext = Configuration.Connection.CreateContext())
            {
                var customlist = clientContext.Web.Lists.GetByTitle(libraryName);
                clientContext.Load(customlist);
                clientContext.ExecuteQuery();
                clientContext.Load(customlist,
                    list => list.UserCustomActions);
                clientContext.ExecuteQuery();
                var collUserCustomAction =
                    customlist.UserCustomActions;
                foreach (var newcustomaAction in collUserCustomAction)
                    if (newcustomaAction.Title.Equals(actionName) &&
                        newcustomaAction.Location.Equals(location))
                    {
                        newcustomaAction.DeleteObject();
                        customlist.Update();
                        break;
                    }

                clientContext.ExecuteQuery();
            }
        }
        //TODO[CR BT]: Extract multiple methods
        public void AddNewCustomAction(string libraryName, string actionName, string location)
        {
            using (var clientContext = Configuration.Connection.CreateContext())
            {
                var customlist = clientContext.Web.Lists.GetByTitle(libraryName);
                clientContext.Load(customlist);
                clientContext.ExecuteQuery();
                var collUserCustomAction =
                    customlist.UserCustomActions;
                foreach (var verifyCustomAction in collUserCustomAction)
                    if (verifyCustomAction.Title.Equals(actionName) &&
                        verifyCustomAction.Location.Equals(location))
                        return;

                var newcustomaAction = collUserCustomAction.Add();
                newcustomaAction.Location = location;
                newcustomaAction.Sequence = 100;
                newcustomaAction.Title = actionName;
                newcustomaAction.Url = CustomActionUrl;
                newcustomaAction.Update();
                clientContext.Load(customlist,
                    list => list.UserCustomActions);

                clientContext.ExecuteQuery();
            }
        }
        //TODO[CR BT]: Format a little bit this code, remove mepty spaces and aling the lines
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
});
       
    }, 500);";
    }
}