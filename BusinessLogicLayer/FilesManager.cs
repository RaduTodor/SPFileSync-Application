namespace BusinessLogicLayer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Constants;
    using Configuration;

    /// <summary>
    /// An instance of FilesManager class can start the sync operations (check and download)
    /// </summary>
    public class FilesManager
    {
        private List<ConnectionConfiguration> connectionConfigurations { get; }

        private ApplicationEnums.ListReferenceProviderType providerType { get; }

        public FilesManager(List<ConnectionConfiguration> configurations, ApplicationEnums.ListReferenceProviderType type)
        {
            connectionConfigurations = configurations;
            providerType = type;
        }

        /// <summary>
        /// Synchronize method iterates all ConnectionConfiguration in connectionConfigurations and creates a new Task
        /// which calls and runs a FileSynchronizer instance Synchronize method.
        /// This is basically the Application Synchronization start.
        /// </summary>
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
