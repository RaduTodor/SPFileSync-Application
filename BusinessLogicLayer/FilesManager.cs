namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Common.ApplicationEnums;
    using Common.Helpers;
    using Configuration;
    using Models;

    /// <summary>
    ///     An instance of FilesManager class can start the sync operations (check and download)
    /// </summary>
    public class FilesManager
    {
        //TODO [CR RT] Why it is not instantiated from ctor?
        //TODO [CR RT] Remove unsused class member
        NotifyIcon notifyIcon = new NotifyIcon();
        NotifyUI notifyUI = new NotifyUI();
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
        ///    //TODO [CR RT] Please add logging when the sync was successfully finished at all and also for a specific configuration.
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
                        notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
                    };
                    fileSync.InternetAccessException += (sender, exception) =>
                    {
                        InternetAccessLost.Invoke(this,true);
                    };
                    fileSync.ProgressUpdate += (sender, number) => { verdicts.FinalizedSyncProccesses[number] = true; };
                    Task.Run(() => fileSync.Synchronize());
                }
                catch (Exception exception)
                {
                    verdicts.FinalizedSyncProccesses[count] = true;
                    MyLogger.Logger.Error(exception, exception.Message);
                    //TODO [CR RT] Remove block {}
                    {
                        notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
                    }
                }
        }
    }
}