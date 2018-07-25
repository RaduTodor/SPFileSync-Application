namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.ApplicationEnums;
    using Common.Helpers;
    using Configuration;
    using Microsoft.SharePoint.Client;
    using Models;

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

        public EventHandler<bool> InternetAccessLost;

        private List<ConnectionConfiguration> ConnectionConfigurations { get; }

        private ListReferenceProviderType ProviderType { get; }

        /// <summary>
        ///     Synchronize method iterates all ConnectionConfiguration in connectionConfigurations and creates a new Task
        ///     which calls and runs a FileSynchronizer instance Synchronize method.
        ///     This is basically the Application Synchronization start.
        /// </summary>
        public void Synchronize(Verdicts verdicts)
        {
            verdicts.FinalizedSyncProccesses = new bool[ConnectionConfigurations.Count];
            var count = -1;
            foreach (var connection in ConnectionConfigurations)
                try
                {
                    verdicts.FinalizedSyncProccesses[++count] = false;
                    var fileSync = new FileSynchronizer(connection, ProviderType, count);
                    fileSync.ExceptionUpdate += (sender, exception) =>
                    {
                        //Notify with bubble
                    };
                    fileSync.InternetAccessException += (sender, exception) =>
                    {
                        //for(int number=0;number<verdicts.FinalizedSyncProccesses.Length;number++)
                        //{
                        //    verdicts.FinalizedSyncProccesses[number] = true;
                        //}
                        //AutomaticFileSynchronizer automaticFileSync = new AutomaticFileSynchronizer(ConnectionConfigurations);
                        //automaticFileSync.InternetAccessInformation += (othersender, information) =>
                        //{
                        //    //Notify with bubble
                        //};
                        //Task.Run(() => automaticFileSync.AutomaticSynchronize());
                        InternetAccessLost?.Invoke(this,true);
                    };
                    fileSync.ProgressUpdate += (sender, number) => { verdicts.FinalizedSyncProccesses[number] = true; };
                    Task.Run(() => fileSync.Synchronize());
                }
                catch (Exception exception)
                {
                    verdicts.FinalizedSyncProccesses[count] = true;
                    MyLogger.Logger.Error(exception, exception.Message);
                    {
                        //Notify with bubble
                    }
                }
        }
    }
}