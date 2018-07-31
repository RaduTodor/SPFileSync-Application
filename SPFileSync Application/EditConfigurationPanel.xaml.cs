
namespace SPFileSync_Application
{
    using BusinessLogicLayer;
    using Common.Helpers;
    using Configuration;
    using Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    public partial class EditConfigurationPanel
    {
        private ConnectionConfiguration _configuration;
        private List<ConnectionConfiguration> _configurations;
        private NotifyUI _notifyUI;
        private ObservableCollection<string> _configurationsName;
        private string _uiPathField = "";
        Window _window;

        public EditConfigurationPanel(ConnectionConfiguration configurationItem, List<ConnectionConfiguration> configurations, Window window)
        {
            InitializeComponent();
            _configurationsName = new ObservableCollection<string>();
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
            _uiPathField = _configuration.DirectoryPath;
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

        private void PopulateObservableCollection()
        {
            foreach (var item in _configurations)
            {
                _configurationsName.Add(item.Connection.UriString);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            ConfigurationUIOperations configurationOperations = new ConfigurationUIOperations();
            ConfigurationWindowModel configurationWindowModel = new ConfigurationWindowModel
            {
                UserName = userNameTextBox.Text,
                Password = passwordText.Password,
                SiteUrl = siteUrlBox.Text,
                Path = _uiPathField,
                SyncInterval = syncTextBox.Text
            };
            WindowNotifyModel windowNotifyModel = new WindowNotifyModel() { NotifyUI = _notifyUI, Window = this };
            var checkIfValid = configurationOperations.EditConfiguration(configurationWindowModel, _configurations, windowNotifyModel, _configuration);
            if (checkIfValid)
            {
                if (_window is Configurations)
                {
                    PopulateObservableCollection();
                    (_window as Configurations).allConfigsList.ItemsSource = _configurationsName;
                }
                Close();
                _window.Show();
            }
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            _uiPathField = PathConfiguration.SetPath(_configuration.DirectoryPath);
            pathLabel.Content = _uiPathField;
        }
    }
}
