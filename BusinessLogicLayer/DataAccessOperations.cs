using Configuration;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class DataAccessOperations
    {
        public DataAccessOperations(ConnectionConfiguration connectionConfiguration, int restOrCsom)
        {
            ConnectionConfiguration = connectionConfiguration;
            Operations = OperationsFactory.GetOperations(restOrCsom);
            Operations.ConnectionConfiguration = connectionConfiguration;
            FilesGetter = new FilesGetter { ConnectionConfiguration = connectionConfiguration };
        }

        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public CRUD_OperationsClass Operations { get; set; }

        public FilesGetter FilesGetter { get; set; }
    }
}
