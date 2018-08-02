namespace Common.Constants
{
    using System.Xml.Linq;

    public static class DataAccessLayerConstants
    {
        public const int WebRequestTimeoutValue = 20000;

        public const int StreamReadBufferBytesDimension = 256;

        public const int LibrarySegmentNumber = 3;

        public const int SyncRetryInterval = 30;
        //TODO [CR BT]: remove unused code
        public const int DefaultConfigurationSyncInterval = 10;

        public const string ContentTypeXml = "application/xml;odata=verbose";

        public const string ContentTypeJson = "application/json;odata=verbose";

        public const string JTokenFirstLayer = "d";

        public const string JTokenSecondLayer = "GetContextWebInformation";

        public const string JTokenThirdLayer = "FormDigestValue";

        public static readonly XNamespace MetadataBaseNamespace = "http://www.w3.org/2005/Atom";

        public static readonly XNamespace MNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        public static readonly XNamespace DNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
    }
}