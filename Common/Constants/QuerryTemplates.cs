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

        public const string ListItemsApi = "_api/web/lists/getbytitle('{0}')/items";

        public const string ListItemByIdApi = "_api/web/lists/getbytitle('{0}')/items({1})";

        public const string NewReferenceItem =
            "{{'__metadata': {{ 'type': 'SP.Data.{0}ListItem' }}, '{1}': {{ '__metadata': {{ 'type': 'SP.FieldUrlValue' }}, 'Url': '{2}','Description': '{3}'}}, '{4}Id' : {5}}}}}";

        public const string ModifiedDateOfUrlApi = "_api/Web/lists/getbytitle('{0}')/items?$select=Modified&$filter=FileRef eq '{1}'";

        public const string SpecificListItemsOfUserApi = "_api/Web/lists/getbytitle('{0}')/items?$select={1}&$filter={2} eq '{3}'";

        public const string Get = "GET";

        public const string Post = "POST";

        public const string Merge = "MERGE";

        public const string Delete = "DELETE";

        public const string Method = "X-HTTP-Method";

        public const string Digest = "X-RequestDigest";

        public const string IfMatch = "IF-MATCH";

        public const string AuthAccept = "X-FORMS_BASED_AUTH_ACCEPTED";

        public const string ContextInfo = "_api/contextinfo";
    }
}
