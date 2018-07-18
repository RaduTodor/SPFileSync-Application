namespace Common.Constants
{
    using System.Xml.Linq;

    public static class DataAccessLayerConstants
    {
        public const int WebRequestTimeoutValue = 20000;

        public const int StreamReadBufferBytesDimension = 256;

        public const int LibrarySegmentNumber = 3;

        public const XNamespace MetadataBaseNamespace = "http://www.w3.org/2005/Atom";

        public const XNamespace MNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        public const XNamespace DNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
    }
}
