namespace Common.Constants
{
    public static class ApiConstants
    {
        public const string ListItemsApi = "_api/web/lists/getbytitle('{0}')/items";

        public const string ListItemByIdApi = "_api/web/lists/getbytitle('{0}')/items({1})";

        public const string NewReferenceItem =
            "{{'__metadata': {{ 'type': 'SP.Data.{0}ListItem' }}, '{1}': {{ '__metadata': {{ 'type': 'SP.FieldUrlValue' }}, 'Url': '{2}','Description': '{3}'}}, '{4}Id' : {5}}}}}";

        public const string ModifiedDateOfUrlApi = "_api/Web/lists/getbytitle('{0}')/items?$select=Modified&$filter=FileRef eq '{1}'";

        public const string SpecificListItemsOfUserApi = "_api/Web/lists/getbytitle('{0}')/items?$select={1}&$filter={2} eq '{3}'";

        public const string ContextInfo = "_api/contextinfo";
    }
}
