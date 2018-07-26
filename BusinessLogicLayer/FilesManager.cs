namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
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
        private NotifyUI _notifyUI = new NotifyUI();
        private System.Timers.Timer _timer = new System.Timers.Timer();
        public FilesManager(List<ConnectionConfiguration> configurations, ListReferenceProviderType type)
        {
            ConnectionConfigurations = configurations;
            ProviderType = type;
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
                try
                {
                    bool syncSuccessful = true;
                    verdicts.FinalizedSyncProccesses[++count] = false;
                    var fileSync = new FileSynchronizer(connection, ProviderType, count);
                    fileSync.ExceptionUpdate += (sender, exception) =>
                    {
                        connection.LastSyncTime = DateTime.Now.Minute;
                        _notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
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
                        connection.LastSyncTime = DateTime.Now.Minute;
                        InternetAccessLost.Invoke(this, true);
                    };
                    Task.Run(() => fileSync.Synchronize());
                    connection.LastSyncTime = DateTime.Now.Minute;
                }
                catch (Exception exception)
                {
                    connection.LastSyncTime = DateTime.Now.Minute;
                    verdicts.FinalizedSyncProccesses[count] = true;
                    MyLogger.Logger.Error(exception, exception.Message);
                    {
                        _notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
                    }
                }
            XmlFileManipulator.Serialize<ConnectionConfiguration>(ConnectionConfigurations);
        }

        private void GeneralSynchronize(Verdicts verdicts, ConnectionConfiguration connection, int count = -1)
        {
            try
            {
                verdicts.FinalizedSyncProccesses[++count] = false;
                var fileSync = new FileSynchronizer(connection, ProviderType, count);
                fileSync.ExceptionUpdate += (sender, exception) =>
                {
                    connection.LastSyncTime = DateTime.Now.Minute;
                    _notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
                };
                fileSync.InternetAccessException += (sender, exception) =>
                {
                    connection.LastSyncTime = DateTime.Now.Minute;
                    InternetAccessLost.Invoke(this, true);
                };
                fileSync.ProgressUpdate += (sender, number) => { verdicts.FinalizedSyncProccesses[number] = true; };
                Task.Run(() => fileSync.Synchronize());
                connection.LastSyncTime = DateTime.Now.Minute;
            }
            catch (Exception exception)
            {
                connection.LastSyncTime = DateTime.Now.Minute;
                verdicts.FinalizedSyncProccesses[count] = true;
                MyLogger.Logger.Error(exception, exception.Message);
                {
                    _notifyUI.BasicNotifyError(Common.Constants.ConfigurationMessages.SyncTitleError, exception.Message);
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

        public void ConfigurationThreadsTimer(System.Windows.Controls.Button syncButton)
        {
            var ticks = TimeSpan.FromMilliseconds(3).Ticks;
            _timer.Interval = ticks;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += (sender, e) => SyncFilesForConfigurationsTime(sender, e, syncButton);
        }

        public void SyncFilesForConfigurationsTime(object sender, System.Timers.ElapsedEventArgs e, System.Windows.Controls.Button syncButton)
        {
            bool checkIfSyncButton = false;
            syncButton.Dispatcher.Invoke(() => { checkIfSyncButton = syncButton.IsEnabled; });
            Verdicts verdicts = new Verdicts
            {
                FinalizedSyncProccesses = new bool[ConnectionConfigurations.Count]
            };
            var count = -1;
            if(checkIfSyncButton )
            {
                var myList = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
                foreach (var connection in myList)
            {
                if (Math.Abs(DateTime.Now.Minute - connection.LastSyncTime) >= connection.SyncTimeSpan.Minutes)
                {
                    GeneralSynchronize(verdicts, connection, count);
                }
            }
            XmlFileManipulator.Serialize<ConnectionConfiguration>(myList);
            }           
        }
    }
}