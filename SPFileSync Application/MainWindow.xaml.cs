namespace SPFileSync_Application
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq.Expressions;
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
            PopulateUIComboBox();
            ApplicationIcon();
            _connectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            if (_connectionConfigurations.Count == 0) SyncButton.IsEnabled = false;

            WaitSync.Visibility = Visibility.Hidden;
        }

        private void ApplicationIcon()
        {
            var notification = new NotifyIcon();
            notification.Icon = new Icon(GeneralUI.GetResourcesFolder(ConfigurationMessages.ResourceFolderAppIcon));
            notification.Visible = true;
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

        private void Sync(object sender, RoutedEventArgs e)
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
                    Dispatcher.Invoke(()=>AutomaticSync());
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

        private void AutomaticSync()
        {
            Task.Run(() =>
            {
                if (InternetAccessHelper.HasInternetAccessAfterRetryInterval())
                    Sync(SyncButton, new RoutedEventArgs());
            });
        }

        private void SetSyncButtonTrue()
        {
            SyncButton.IsEnabled = true;
            WaitSync.Visibility = Visibility.Hidden;
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