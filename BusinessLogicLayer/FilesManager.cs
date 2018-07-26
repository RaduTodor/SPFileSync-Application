namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Common.ApplicationEnums;
    using Common.Constants;
    using Common.Helpers;
    using Configuration;
    using Models;

    /// <summary>
    ///     An instance of FilesManager class can start the sync operations (check and download)
    /// </summary>
    public class FilesManager
    {
        //TODO [CR BT] Why it is not instantiated from ctor?
        //TODO [CR BT] Remove unsused class member
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
        public void Synchronize(Verdicts verdicts)
        {
            verdicts.FinalizedSyncProccesses = new bool[ConnectionConfigurations.Count];
            var count = -1;
            foreach (var connection in ConnectionConfigurations)
                try
                {
                    bool syncSuccessful = true;
                    verdicts.FinalizedSyncProccesses[++count] = false;
                    var fileSync = new FileSynchronizer(connection, ProviderType, count);
                    fileSync.ExceptionUpdate += (sender, exception) =>
                    {
                        notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
                        syncSuccessful = false;
                    };
                    fileSync.InternetAccessException += (sender, exception) =>
                    {
                        InternetAccessLost.Invoke(this, true);
                        syncSuccessful = false;
                    };
                    fileSync.ProgressUpdate += (sender, number) =>
                    {
                        if (syncSuccessful)
                        {
                            MyLogger.Logger.Trace(string.Format(DefaultTraceMessages.ConfigurationSyncFinishedSuccessfully,
                                connection.Connection.Uri));
                        }
                        else
                        {
                            MyLogger.Logger.Error(string.Format(DefaultExceptionMessages.ConfigurationSyncFinishedUnssuccesful,
                                connection.Connection.Uri));
                        }

                        verdicts.FinalizedSyncProccesses[number] = true;
                    };
                    Task.Run(() => fileSync.Synchronize());
                }
                catch (Exception exception)
                {
                    verdicts.FinalizedSyncProccesses[count] = true;
                    MyLogger.Logger.Error(exception, exception.Message);
                    notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
                }
        }
    }
}