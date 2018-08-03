namespace BusinessLogicLayer
{
    using Common.ApplicationEnums;
    using Configuration;
    using DataAccessLayer;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FileSearcher
    {
        private FileOperationProvider FileOperationProvider { get; }
        private BaseListReferenceProvider ListReferenceProvider { get; }

        public FileSearcher(ConnectionConfiguration configuration, ListReferenceProviderType type)
        {
            FileOperationProvider = new FileOperationProvider(configuration);
            ListReferenceProvider = OperationsFactory.GetOperations(type);
            ListReferenceProvider.ConnectionConfiguration = configuration;
        }
        //TODO[CR BT]: Remove unused code
        public FileSearcher() { }

        public Task<Dictionary<string, string>> SearchSpFiles(string item)
        {
           return Task.Run(() => SearchFiles(item));
        }
        //TODO[CR BT]: Remove empty lines
        private Dictionary<string, string> SearchFiles(string item)
        {
            
            return ListReferenceProvider.SearchSPFiles(item);
        }

    }
}
