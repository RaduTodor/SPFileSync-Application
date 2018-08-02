﻿namespace SPFileSync_Application
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
        private List<ConnectionConfiguration> _connectionConfigurations;
        FilesManager _fileManager;
        NotifyUI _notifyUI;

        public MainWindow()
        {
            InitializeComponent();
            Hide();
            _connectionConfigurations = new List<ConnectionConfiguration>();
            _notifyUI = new NotifyUI(this, ListBox1);
            PopulateUIComboBox();
            CreateApplicationIcon();    
            _connectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            _fileManager = new FilesManager(_connectionConfigurations, GetProviderType(configComboBox.SelectedItem.ToString()), _notifyUI);         
            if (_connectionConfigurations.Count == 0) SyncButton.IsEnabled = false;
            _fileManager.TimerSyncronize(SyncButton, WaitAutomaticSync);
            WaitSync.Visibility = Visibility.Hidden;            
            WaitAutomaticSync.Visibility = Visibility.Hidden;     
        }

        private ContextMenu NotificationIconContextMenu()
        {
            var context = new ContextMenu();
            var syncItem = new MenuItem
            {
                Index = 0,
                Text = ConfigurationMessages.Sync
            };
            syncItem.Click += SyncItemClick;
            var exitItem = new MenuItem
            {
                Index = 1,
                Text = ConfigurationMessages.Exit
            };
            exitItem.Click += ExitItemClick;
            context.MenuItems.Add(syncItem);
            context.MenuItems.Add(exitItem);
            return context;
        }

        private void CreateApplicationIcon()
        {
            var notification = new NotifyIcon();
            notification.Icon =
                new Icon(PathConfiguration.GetApplicationDirectory(
                    ConfigurationMessages.ResourceFolderAppIcon));
            notification.Visible = true;
            notification.ContextMenu = NotificationIconContextMenu();
            notification.Text = ConfigurationMessages.AppName;
            notification.DoubleClick += (sender, e) => ShowWindow(sender, e);
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void ExitItemClick(object sender, EventArgs e)
        {
            Close();
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
                _fileManager.Synchronize(verdicts);
                _fileManager.InternetAccessLost += (senderObject, truthValue) =>
                {
                    _notifyUI.NotifyUserWithTrayBarBalloon(ConfigurationMessages.InternetAccesError, ConfigurationMessages.InternetAccesErrorMessage);
                    Dispatcher.Invoke(() => AutomaticSync());
                };

                _notifyUI.NotifyUserWithTrayBarBalloon(ConfigurationMessages.SyncIsActive, ConfigurationMessages.SyncActiveMessage);
                var syncProgressProvider = new SyncProgressManager();
                syncProgressProvider.ProgressUpdate += (s, verdict) =>
                {
                    Dispatcher.Invoke(() => SetSyncButtonTrue());
                };
                Task.Run(() => { syncProgressProvider.CheckSyncProgress(syncProgressProvider, verdicts); });
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
                        Dispatcher.Invoke(() => Sync(SyncButton, new RoutedEventArgs()));
                    RetryThreadOn = false;
                    Dispatcher.Invoke(() => SetSyncButtonTrue());
                });
            }
        }

        private void SetSyncButtonTrue()
        {
            if (!RetryThreadOn)
            {
                SyncButton.IsEnabled = true;
                WaitSync.Visibility = Visibility.Hidden;
                _notifyUI.NotifyUserWithTrayBarBalloon(ConfigurationMessages.SyncEnded, ConfigurationMessages.SyncEndMessage);
            }
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

        private void ListOperations(object sender, RoutedEventArgs e)
        {
            _connectionConfigurations = XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            var window = new ReferenceListOperationsWindow(_connectionConfigurations);
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new UpdatedFilesWindow();
            window.Show();
        }

        private void SearchFiles(object sender, RoutedEventArgs e)
        {
            SearchWindow window = new SearchWindow(_connectionConfigurations);
            window.Show();
        }
    }
}