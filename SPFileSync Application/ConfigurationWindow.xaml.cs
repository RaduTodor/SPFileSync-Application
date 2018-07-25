
namespace SPFileSync_Application
{
    using BusinessLogicLayer;
    using Common.Helpers;
    using Configuration;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Forms;

    public partial class ConfigurationWindow
    {

        private string _path;       
        private ConnectionConfiguration _configuration;
        private NotifyUI _notifyUI;
        private List<ConnectionConfiguration> _configurations;
        public ConfigurationWindow(List<ConnectionConfiguration> connectionConfigurations)
        {
            InitializeComponent();
            _notifyUI = new NotifyUI(this,errorList);
            _configurations = connectionConfigurations;
            mainWindow = window;
            InitializeUI();
        }

        private void InitializeUI()
        {
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxCsom);
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            _path = PathConfiguration.SetPath();
        }
       
        private void Save(object sender, RoutedEventArgs e)
        {
            ConfigurationUIOperations configurationOperations = new ConfigurationUIOperations();
            ConfigurationWindowModel configurationWindowModel = new ConfigurationWindowModel { UserName = userNameTextBox.Text, Password = passwordText.Password, SiteUrl = siteUrlBox.Text,
                Path = _path, ListName = listTextBox.Text, UrlColumn = urlColumnTextBox.Text, UserColumn = userColumnTextBox.Text,SyncInterval = syncTextBox.Text };
            WindowNotifyModel windowNotifyModel = new WindowNotifyModel() {NotifyUI = _notifyUI, Window = this };
            configurationOperations.AddNewConfiguration(configurationWindowModel, _configurations,windowNotifyModel);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddListWithColumns(object sender, RoutedEventArgs e)
        {
            Connection.CheckConfiguration(ref _configuration);
            ListWithColumns window = new ListWithColumns(_configuration.ListsWithColumnsNames);
            window.Show();
        }
    }
}
