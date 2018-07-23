namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.ApplicationEnums;
    using Common.Helpers;
    using Configuration;

    /// <summary>
    ///     An instance of FilesManager class can start the sync operations (check and download)
    /// </summary>
    public class FilesManager
    {
        public FilesManager(List<ConnectionConfiguration> configurations, ListReferenceProviderType type)
        {
            ConnectionConfigurations = configurations;
            ProviderType = type;
        }

        private List<ConnectionConfiguration> ConnectionConfigurations { get; }

        private ListReferenceProviderType ProviderType { get; }

        /// <summary>
        ///     Synchronize method iterates all ConnectionConfiguration in connectionConfigurations and creates a new Task
        ///     which calls and runs a FileSynchronizer instance Synchronize method.
        ///     This is basically the Application Synchronization start.
        /// </summary>
        public void Synchronize()
        {
            foreach (var connection in ConnectionConfigurations)
                try
                {
                    var fileSync = new FileSynchronizer(connection, ProviderType);
                    fileSync.ExceptionUpdate += (sender, exception) =>
                    {
                        //Notify with bubble
                    };
                    Task.Run(() => fileSync.Synchronize());
                }
                catch (Exception exception)
                {
                    MyLogger.Logger.Error(exception, exception.Message);
                    {
                        //Notify with bubble
                    }
                }
        }
    }
}