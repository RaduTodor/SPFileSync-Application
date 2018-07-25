
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

    //TODO [CR BT] Resolve usings
    public partial class EditConfigurationPanel
    {
        private ConnectionConfiguration _configuration;
        private List<ConnectionConfiguration> _configurations;
        private NotifyUI _notifyUI;
        private string _path = "";
        Window _window;

        public EditConfigurationPanel(ConnectionConfiguration configurationItem, List<ConnectionConfiguration> configurations, Window window)
        {
            InitializeComponent();
            InitializeFields(configurationItem, configurations, window);
            UpdateUI();
        }

        private void InitializeFields(ConnectionConfiguration configurationItem, List<ConnectionConfiguration> configurations, Window window)
        {
            _notifyUI = new NotifyUI(this, errorList);
            _configuration = configurationItem;
            this._configurations = configurations;
            this._window = window;
        }

        private void UpdateUI()
        {
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxCsom);
            _path = _configuration.DirectoryPath;
            siteUrlBox.Text = _configuration.Connection.Uri.ToString();
            syncTextBox.Text = _configuration.SyncTimeSpan.TotalMinutes.ToString();
            userNameTextBox.Text = _configuration.Connection.Credentials.UserName;
            passwordText.Password = _configuration.Connection.Credentials.Password;
            pathLabel.Content = _configuration.DirectoryPath;
        }

        private void ListEdit(object sender, RoutedEventArgs e)
        {
            ConfigurationListsEdit window = new ConfigurationListsEdit(_configuration.ListsWithColumnsNames);
            window.Show();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            _window.Show();
            Close();
        }


        
        private void Save(object sender, RoutedEventArgs e)
        {          
            ConfigurationUIOperations configurationOperations = new ConfigurationUIOperations();
            ConfigurationWindowModel configurationWindowModel = new ConfigurationWindowModel
            {
                UserName = userNameTextBox.Text,
                Password = passwordText.Password,
                SiteUrl = siteUrlBox.Text,
                Path = _path,
                SyncInterval = syncTextBox.Text
            };
            WindowNotifyModel windowNotifyModel = new WindowNotifyModel() {NotifyUI = _notifyUI, Window = this };
            var checkIfValid = configurationOperations.EditConfiguration(configurationWindowModel, _configurations, windowNotifyModel, _configuration);
            if(checkIfValid)
            {
                Close();
                _window.Show();
            }                    
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            //TODO [CR BT] Remove redundant path
            _path = Common.Helpers.PathConfiguration.SetPath(_configuration.DirectoryPath);
            pathLabel.Content = _path;
        }
    }
}
