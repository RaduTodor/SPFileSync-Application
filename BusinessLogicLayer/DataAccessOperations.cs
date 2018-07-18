using Configuration;
using DataAccessLayer;

namespace BusinessLogicLayer
{   //TODO [CR RT]: To be dicided if the class is needed
    //TODO [CR RT]: SRP violation, class handles operations, keeps the conection and also download files -> to be split in different managers
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
