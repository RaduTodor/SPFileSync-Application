namespace SPFileSync_Application
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using BusinessLogicLayer;
    using Common.ApplicationEnums;
    using Common.Constants;
    using Common.Helpers;
    using Configuration;
    using Models;

    public partial class MainWindow
    {
        private List<ConnectionConfiguration> _connectionConfigurations = new List<ConnectionConfiguration>();

        public MainWindow()
        {
            InitializeComponent();
            Hide();
            PopulateUIComboBox();
            ApplicationIcon();
            _connectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            if (_connectionConfigurations.Count == 0) SyncButton.IsEnabled = false;

            WaitSync.Visibility = Visibility.Hidden;
        }

        private void ApplicationIcon()
        {
            var notification = new NotifyIcon();
            notification.Icon =
                new Icon(Common.Helpers.PathConfiguration.GetResourcesFolder(
                    ConfigurationMessages.ResourceFolderAppIcon));
            notification.Visible = true;
            var notificationContextStrip = new ContextMenuStrip();
            var context = new ContextMenu();
            var syncItem = new MenuItem
            {
                Index = 0,
                Text = "Sync"
            };
            syncItem.Click += SyncItemClick;
            context.MenuItems.Add(syncItem);
            notification.ContextMenu = context;
            notification.Text = ConfigurationMessages.AppName;
            notification.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
        }

        protected override void OnClosed(EventArgs e)
        {
            Environment.Exit(1);
            base.OnClosed(e);
        }

        private void SyncItemClick(object sender, EventArgs e)
        {
            SyncFiles();
        }

        private void PopulateUIComboBox()
        {
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxCsom);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Environment.Exit(1);
            base.OnClosing(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            base.OnStateChanged(e);
        }

        private void AddConfig(object sender, RoutedEventArgs e)
        {
            var window = new ConfigurationWindow(_connectionConfigurations, this);
            window.Show();
        }

        private void SyncFiles()
        {
            if (InternetAccessHelper.HasInternetAccess())
            {
                SyncButton.IsEnabled = false;
                WaitSync.Visibility = Visibility.Visible;
                var verdicts = new Verdicts();
                var fileManager = new FilesManager(_connectionConfigurations,
                    GetProviderType(configComboBox.SelectedItem.ToString()));
                fileManager.Synchronize(verdicts);
                fileManager.InternetAccessLost += (senderObject, truthValue) =>
                {
                    //Notify with bubble
                    Dispatcher.Invoke(() => AutomaticSync());
                };

                //Notify with bubble that the sync is currently on

                var syncProgressProvider = new SyncProgressProvider();
                syncProgressProvider.ProgressUpdate += (s, verdict) =>
                {
                    Dispatcher.Invoke(() => SetSyncButtonTrue());
                };
                Task.Run(() => { syncProgressProvider.Operation(syncProgressProvider, verdicts); });
            }
            else
            {
                AutomaticSync();
            }
        }

        private bool RetryThreadOn = false;

        private void AutomaticSync()
        {
            if (!RetryThreadOn)
            {
                SyncButton.IsEnabled = false;
                RetryThreadOn = true;
                Task.Run(() =>
                {
                    if (InternetAccessHelper.HasInternetAccessAfterRetryInterval())
                        this.Dispatcher.Invoke(()=>Sync(SyncButton, new RoutedEventArgs()));
                    RetryThreadOn = false;
                });
            }
        }

        private void SetSyncButtonTrue()
        {
            SyncButton.IsEnabled = true;
            WaitSync.Visibility = Visibility.Hidden;
        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            SyncFiles();
        }

        private void SeeConfigurations(object sender, RoutedEventArgs e)
        {
            _connectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            var window = new Configurations(_connectionConfigurations, this);
            window.Show();
        }

        private static ListReferenceProviderType GetProviderType(string choice)
        {
            switch (choice)
            {
                case ConfigurationMessages.ComboBoxRest:
                    return ListReferenceProviderType.Rest;

                case ConfigurationMessages.ComboBoxCsom:
                    return ListReferenceProviderType.Csom;

                default:
                    return ListReferenceProviderType.Rest;
            }
        }
    }
}