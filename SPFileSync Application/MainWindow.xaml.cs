

namespace SPFileSync_Application
{
    using Configuration;
    using System.Windows;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.IO;
    using System.Drawing;
    using System;
    using BusinessLogicLayer;
    using Common.Constants;
    using System.ComponentModel;

    public partial class MainWindow 
    {
        private List<ConnectionConfiguration> _connectionConfigurations = new List<ConnectionConfiguration>();       
        public MainWindow()
        {
            InitializeComponent();
            Hide();
            PopulateUIComboBox();
            ApplicationIcon();
            _connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();
        }

        private void ApplicationIcon()
        {
            NotifyIcon notification = new NotifyIcon();
            notification.Icon = new Icon(Common.Helpers.PathConfiguration.GetResourcesFolder(ConfigurationMessages.ResourceFolderAppIcon));
            notification.Visible = true;
            ContextMenuStrip notificationContextStrip = new ContextMenuStrip();
            ContextMenu context = new ContextMenu();
            MenuItem syncItem = new MenuItem
            {
                Index = 0,
                Text = "Sync"
            };
            syncItem.Click += SyncItemClick;
            context.MenuItems.Add(syncItem);
            notification.ContextMenu = context;
            notification.Text = ConfigurationMessages.AppName;
            notification.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
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
            ConfigurationWindow window = new ConfigurationWindow(_connectionConfigurations);
            window.Show();
        }

        private void SyncFiles()
        {
            FilesManager fileManager = new FilesManager(_connectionConfigurations, GetProviderType(configComboBox.SelectedItem.ToString()));
            fileManager.Synchronize();
        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            SyncFiles();
        }

        private void SeeConfigurations(object sender, RoutedEventArgs e)
        {
            _connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            Configurations window = new Configurations(_connectionConfigurations);
            window.Show();
        }
      
        private static Common.ApplicationEnums.ListReferenceProviderType GetProviderType(string choice)
        {
            switch (choice)
            {
                case ConfigurationMessages.ComboBoxRest:
                    return Common.ApplicationEnums.ListReferenceProviderType.Rest;

                case ConfigurationMessages.ComboBoxCsom:
                    return Common.ApplicationEnums.ListReferenceProviderType.Csom;

                default:
                    return Common.ApplicationEnums.ListReferenceProviderType.Rest;
            }
        }
    }
}
