namespace SPFileSync_Application
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Forms;
    using Common.Constants;
    using Common.Exceptions;
    using Common.Helpers;
    using Configuration;
    using Models;

    public partial class ConfigurationWindow
    {
        private ConnectionConfiguration _configuration;
        private readonly List<ConnectionConfiguration> _configurations;
        private readonly GeneralUI _generalUI;
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        private string _path;
        private readonly MainWindow mainWindow;

        public ConfigurationWindow(List<ConnectionConfiguration> connectionConfigurations, MainWindow window)
        {
            InitializeComponent();
            _generalUI = new GeneralUI(this);
            _configurations = connectionConfigurations;
            mainWindow = window;
            InitializeUI();
        }

        private void InitializeUI()
        {
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxCsom);
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            _path = PathConfiguration.SetPath();
        }

        //TODO [CR BT] : Duplicate code in catch. Extract class eg. NotifyUI and send corresponding parameters.
        //TODO [CR BT] : Extract try logic and move it into Business Logic. Use the UI textboxes model created above and send it to this class. Split this logic in multiple methods. This logic is making two different operations: create a connection and add a list to configuration which will be serialized .
        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                var connection = new Connection
                {
                    Credentials = new Credentials {UserName = userNameTextBox.Text, Password = passwordText.Password},
                    Uri = new Uri(siteUrlBox.Text)
                };
                var minutes = int.Parse(syncTextBox.Text);
                connection.Login();
                GeneralUI.checkConfiguration(ref _configuration);
                _configuration.Connection = connection;
                _configuration.DirectoryPath = _path;
                _configuration.SyncTimeSpan = TimeSpan.FromMinutes(minutes);
                var list = new ListWithColumnsName
                {
                    ListName = listTextBox.Text,
                    UrlColumnName = urlColumnTextBox.Text,
                    UserColumnName = userColumnTextBox.Text
                };
                if (_configuration.ListsWithColumnsNames == null)
                    _configuration.ListsWithColumnsNames = new List<ListWithColumnsName>();
                _configuration.ListsWithColumnsNames.Add(list);
                _configurations.Add(_configuration);
                XmlFileManipulator.Serialize(_configurations);
                if (mainWindow.SyncButton.IsEnabled == false)
                    mainWindow.SyncButton.IsEnabled = true;
                Close();
            }
            catch (UriFormatException uriException)
            {
                _generalUI.NotifyError(_notifyIcon, ConfigurationMessages.BadConfigurationTitle,
                    ConfigurationMessages.InvalidSiteUrl);
                _generalUI.AddToListButton(ConfigurationMessages.InvalidSiteUrl);
                MyLogger.Logger.Debug(uriException);
            }
            catch (LoginException webException)
            {
                _generalUI.NotifyError(_notifyIcon, ConfigurationMessages.BadConfigurationTitle,
                    ConfigurationMessages.CredentialsError);
                _generalUI.AddToListButton(ConfigurationMessages.CredentialsError);

                MyLogger.Logger.Debug(webException);
            }
            catch (Exception exception)
            {
                _generalUI.NotifyError(_notifyIcon, ConfigurationMessages.BadConfigurationTitle,
                    ConfigurationMessages.GeneralConfigError);
                _generalUI.AddToListButton(ConfigurationMessages.GeneralConfigError);
                MyLogger.Logger.Debug(exception);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddListWithColumns(object sender, RoutedEventArgs e)
        {
            GeneralUI.checkConfiguration(ref _configuration);
            var window = new ListWithColumns(_configuration.ListsWithColumnsNames);
            window.Show();
        }
    }
}