using DataAccessLayer;
using Microsoft.SharePoint.Client;

namespace BusinessLogicLayer
{
    public class ListOperations
    {
        public static void DownloadFilesOfUser(DataAccessOperations dataAccessOperations)
        {
            var listOfItems = dataAccessOperations.Operations.GetListItems(dataAccessOperations.Operations.ListName);
            string currentUserName = dataAccessOperations.Operations.GetCurrentUserName();
            foreach (var item in listOfItems)
            {
                FieldUserValue itemUser = (FieldUserValue)item["User"];
                if (itemUser != null)
                {
                    if (((FieldUserValue)item["User"]).LookupValue == currentUserName)
                    {
                        string url = ((Microsoft.SharePoint.Client.FieldUrlValue)(item["URL"])).Url;
                        dataAccessOperations.FilesGetter.DownloadFile(url, dataAccessOperations.ConnectionConfiguration.DirectoryPath);
                    }
                }
            }
        }
    }
}
