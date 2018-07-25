

namespace SPFileSync_Application
{
    using Configuration;
    using System.Windows;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.IO;
    using System.Drawing;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms.VisualStyles;
    using BusinessLogicLayer;
    using Common.Constants;
    using System.ComponentModel;

    public partial class MainWindow
    {
        private List<ConnectionConfiguration> _connectionConfigurations = new List<ConnectionConfiguration>();

        public MainWindow()
        {
            InitializeComponent();
            PopulateUIComboBox();
            ApplicationIcon();
            _connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            if (_connectionConfigurations.Count == 0)
            {
                SyncButton.IsEnabled = false;
            }

            WaitSync.Visibility = Visibility.Hidden;
        }

        private void ApplicationIcon()
        {
            NotifyIcon notification = new NotifyIcon();
            notification.Icon = new Icon(GeneralUI.GetResourcesFolder(ConfigurationMessages.ResourceFolderAppIcon));
            notification.Visible = true;
            notification.Text = ConfigurationMessages.AppName;
            notification.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
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
            ConfigurationWindow window = new ConfigurationWindow(_connectionConfigurations, this);
            window.Show();
        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            SyncButton.IsEnabled = false;
            WaitSync.Visibility = Visibility.Visible;
            Verdicts verdicts = new Verdicts();
            FilesManager fileManager = new FilesManager(_connectionConfigurations,
                GetProviderType(configComboBox.SelectedItem.ToString()));
            fileManager.Synchronize(verdicts);

            //Notify with bubble that the sync is currently on

            SyncProgressProvider syncProgressProvider = new SyncProgressProvider();
            syncProgressProvider.ProgressUpdate += (s, verdict) =>
            {
                this.Dispatcher.Invoke(()=>SetSyncButtonTrue());
            };
            Task.Run(() =>
            {
                syncProgressProvider.Operation(syncProgressProvider,verdicts);               
            });
        }

        private void SetSyncButtonTrue()
        {
            SyncButton.IsEnabled = true;
            WaitSync.Visibility = Visibility.Hidden;
        }

        private void SeeConfigurations(object sender, RoutedEventArgs e)
        {
            _connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            Configurations window = new Configurations(_connectionConfigurations, this);
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
