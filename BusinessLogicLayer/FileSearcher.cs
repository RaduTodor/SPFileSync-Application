using Common.ApplicationEnums;
using Configuration;
using DataAccessLayer;
//TODO CR: Please remove unused using and add the rest inside the namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
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

        public FileSearcher() { }

        public Task<Dictionary<string, string>> SearchSpFiles(string item)
        {
           return Task.Run(() => SearchFiles(item));
        }

        private Dictionary<string, string> SearchFiles(string item)
        {
            
            return ListReferenceProvider.SearchSPFiles(item);
        }

    }
}
