namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
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
        private NotifyUI _notifyUI;
        private System.Timers.Timer _timer;

        public FilesManager(List<ConnectionConfiguration> configurations, ListReferenceProviderType type, NotifyUI notifyUI)
        {
            ConnectionConfigurations = configurations;
            ProviderType = type;
            _notifyUI = notifyUI;
            _timer = new System.Timers.Timer();
        }

        public EventHandler<bool> InternetAccessLost;

        private List<ConnectionConfiguration> ConnectionConfigurations { get; set; }

        private ListReferenceProviderType ProviderType { get; }

        /// <summary>
        ///     Synchronize method iterates all ConnectionConfiguration in connectionConfigurations and creates a new Task
        ///     which calls and runs a FileSynchronizer instance Synchronize method.
        ///     This is basically the Application Synchronization start.
        /// </summary>
        /// 
        public void Synchronize(Verdicts verdicts)
        {
            verdicts.FinalizedSyncProccesses = new bool[ConnectionConfigurations.Count];
            var count = -1;
            foreach (var connection in ConnectionConfigurations)
            {
                SynchronizeConfigurations(verdicts, connection, count);
            }
            XmlFileManipulator.Serialize<ConnectionConfiguration>(ConnectionConfigurations);
        }
        
        private void SynchronizeConfigurations(Verdicts verdicts, ConnectionConfiguration connection, int syncThreadNumber)
        {
            try
            {
                bool syncSuccessful = true;
                verdicts.FinalizedSyncProccesses[++syncThreadNumber] = false;
                var fileSync = new FileSynchronizer(connection, ProviderType, syncThreadNumber);
                fileSync.ExceptionUpdate += (sender, exception) =>
                {
                    connection.LastSyncTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);
                    _notifyUI.NotifyUserWithTrayBarBalloon(ConfigurationMessages.SyncTitleError, exception.Message);
                    syncSuccessful = false;
                };
                fileSync.InternetAccessException += (sender, exception) =>
                {
                    connection.LastSyncTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);
                    InternetAccessLost.Invoke(this, true);
                    syncSuccessful = false;
                };
                fileSync.ProgressUpdate += (sender, number) =>
                {
                    if (syncSuccessful)
                    {
                        LoggerManager.Logger.Trace(string.Format(DefaultTraceMessages.ConfigurationSyncFinishedSuccessfully,
                            connection.Connection.Uri));
                    }
                    else
                    {
                        LoggerManager.Logger.Warn(string.Format(DefaultExceptionMessages.ConfigurationSyncFinishedUnssuccesful,
                            connection.Connection.Uri));
                    }
                    verdicts.FinalizedSyncProccesses[number] = true;
                    connection.LastSyncTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);
                };
                Task.Run(() => fileSync.Synchronize());
                connection.LastSyncTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);
            }
            catch (Exception exception)
            {
                connection.LastSyncTime = DateTime.Now.AddSeconds(-DateTime.Now.Second);
                verdicts.FinalizedSyncProccesses[syncThreadNumber] = true;
                LoggerManager.Logger.Error(exception, exception.Message);
                {
                    _notifyUI.NotifyUserWithTrayBarBalloon(ConfigurationMessages.SyncTitleError, exception.Message);
                }
            }
        }
        /// <summary>
        ///    Sync automatically every configuration after an interval
        /// </summary>
        public void TimerSyncronize(System.Windows.Controls.Button syncButton)
        {
            Thread thread = new Thread(() => ConfigurationThreadsTimer(syncButton));
            thread.Start();
        }
        /// <summary>
        ///   This method sets the timer and at every 60 seconds calls SyncFilesForConfigurationsTime which check for last sync date and if sync button is enabled.
        /// </summary>
        private void ConfigurationThreadsTimer(System.Windows.Controls.Button syncButton)
        {
            var ticks = TimeSpan.FromMilliseconds(60).Ticks;
            _timer.Interval = ticks;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += (sender, e) => SyncFilesForConfigurationsTime(syncButton);
        }

        /// <summary>
        ///   This method creates a task for every configuration and synchronize them.
        /// </summary>
        private void SyncFilesForConfigurationsTime(System.Windows.Controls.Button syncButton)
        {
            bool checkIfSyncButton = false;
            syncButton.Dispatcher.Invoke(() => { checkIfSyncButton = syncButton.IsEnabled; });
            Verdicts verdicts = new Verdicts
            {
                FinalizedSyncProccesses = new bool[ConnectionConfigurations.Count]
            };
            var syncThreadNumber = -1;
            if (checkIfSyncButton)
            {
                ConnectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
                foreach (var connection in ConnectionConfigurations)
                {
                    if (Math.Abs(DateTime.Now.Ticks - connection.LastSyncTime.Ticks) >= connection.SyncTimeSpan.Ticks)
                    {
                        syncButton.Dispatcher.Invoke(() => { syncButton.IsEnabled = false; });
                        SynchronizeConfigurations(verdicts, connection, syncThreadNumber);
                    }
                }
                XmlFileManipulator.Serialize<ConnectionConfiguration>(ConnectionConfigurations);
                syncButton.Dispatcher.Invoke(() => { syncButton.IsEnabled = true; });
            }
        }
    }
}