namespace BusinessLogicLayer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Constants;
    using Configuration;

    //TODO [CR RT]: Add class and methods documentation
    public class FilesManager
    {
        private List<ConnectionConfiguration> connectionConfigurations { get; }

        private ApplicationEnums.ListReferenceProviderType providerType { get; }

        public FilesManager(List<ConnectionConfiguration> configurations, ApplicationEnums.ListReferenceProviderType type)
        {
            connectionConfigurations = configurations;
            providerType = type;
        }

        public void Synchronize()
        {
            foreach (var connection in connectionConfigurations)
            {
                var fileSync = new FileSynchronizer(connection,providerType);
                var t = Task.Run(() => fileSync.Synchronize());
            }
        }
    }
}
