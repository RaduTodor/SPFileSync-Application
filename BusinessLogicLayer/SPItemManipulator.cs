using SP = Microsoft.SharePoint.Client;

namespace BusinessLogicLayer
{
    public class SPItemManipulator
    {
        public static string GetValueURL(SP.ListItem item, string columnName)
        {
            SP.FieldUrlValue value = (SP.FieldUrlValue)item[columnName];
            return value.Url;
        }
    }
}
