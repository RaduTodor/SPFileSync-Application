using Configuration;

namespace DataAccessLayer
{
    public class DataAccessOperations
    {
        public DataAccessOperations(ConnectionConfiguration connectionConfiguration)
        {
            ConnectionConfiguration = connectionConfiguration;
            Operations = new CRUD_Operations { ConnectionConfiguration = connectionConfiguration};
            FilesGetter = new FilesGetter { ConnectionConfiguration = connectionConfiguration};
        }

        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public CRUD_Operations Operations { get; set; }

        public FilesGetter FilesGetter { get; set; }
    }
}
