namespace Common.Constants
{
    public static class QuerryTemplates
    {
        public const string AllListItems = "<View Scope='RecursiveAll'>" +
                                           "<Query>" +
                                           "<Where>" +
                                           "<Eq>" +
                                           "<FieldRef Name='FSObjType' />" +
                                           "<Value Type='Integer'>0</Value>" +
                                           "</Eq>" +
                                           "</Where>" +
                                           "</Query>" +
                                           "</View>";

        public const string SearchItems = "<Where>" +
            "<Contains>" +
            "<FieldRef Name=\"AssignedTo\" />" +
            "<Value Type='Text'>" +
            "'{0}'" +
            "</Value>" +
            "</Contains>" +
            "</Where>";
    }
}