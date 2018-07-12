using DataAccessLayer;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer
{
    public class ListOperations
    {
        public static void DownloadFilesOfUser(DataAccessOperations dataAccessOperations)
        {
            List<ListItem> items = GetAllUserItems(dataAccessOperations);
            foreach (ListItem item in items)
            {
                string url = SPItemManipulator.GetValueURL(item, "URL");
                dataAccessOperations.FilesGetter.DownloadFile(url, dataAccessOperations.ConnectionConfiguration.DirectoryPath);
            }
        }

        public static List<ListItem> GetAllUserItems(DataAccessOperations dataAccessOperations)
        {
            var listOfItems = dataAccessOperations.Operations.GetListItems().ToList();
            string currentUserName = dataAccessOperations.Operations.GetCurrentUserName();
            List<ListItem> allUserItems = new List<ListItem>();
            foreach (var item in listOfItems)
            {
                FieldUserValue itemUser = (FieldUserValue)item["User"];
                if (itemUser != null)
                {
                    if (((FieldUserValue)item["User"]).LookupValue == currentUserName)
                    {
                        allUserItems.Add(item);
                    }
                }
            }
            return allUserItems;
        }
    }
}
