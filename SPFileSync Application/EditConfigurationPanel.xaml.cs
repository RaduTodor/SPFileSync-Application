
namespace SPFileSync_Application
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Forms;

    public partial class EditConfigurationPanel
    {
        private ConnectionConfiguration _configuration;
        private List<ConnectionConfiguration> _configurations;
        private GeneralUI _generalUI;
        private NotifyIcon _notifyIcon = new NotifyIcon();
        private string _path = "";
        private List<ConnectionConfiguration> _initialConfigurations;
        Window _window;

        public EditConfigurationPanel(ConnectionConfiguration configurationItem, List<ConnectionConfiguration> configurations, Window window)
        {
            InitializeComponent();
            InitializeFields(configurationItem, configurations, window);
            _initialConfigurations = new List<ConnectionConfiguration>(_configurations);
            UpdateUI();
        }

        private void InitializeFields(ConnectionConfiguration configurationItem, List<ConnectionConfiguration> configurations, Window window)
        {
            _generalUI = new GeneralUI(this);
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
            _configurations = new List<ConnectionConfiguration>(_initialConfigurations);
            _window.Show();
            Close();
        }




        //TODO [CR BT] : Duplicate code in catch. Extract class eg. NotifyUI and send corresponding parameters.
        //TODO [CR BT] : Extract try logic and move it into Business Logic. Use the UI textboxes model created above and send it to this class. 
        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                double minutes = 0;
                var checkSyncTime = double.TryParse(syncTextBox.Text, out minutes);
                Connection connection = new Connection() { Credentials = new Models.Credentials { UserName = userNameTextBox.Text, Password = passwordText.Password }, Uri = new Uri(siteUrlBox.Text) };
                _configuration.Connection = connection;
                _configuration.SyncTimeSpan = TimeSpan.FromMinutes(minutes);
                _configuration.Connection.Login();
                _configuration.DirectoryPath = _path;
                Common.Helpers.XmlFileManipulator.Serialize(_configurations);
                _window.Show();
                Close();

            }
            catch (UriFormatException uriException)
            {

                _generalUI.NotifyError(_notifyIcon, Common.Constants.ConfigurationMessages.BadConfigurationTitle, Common.Constants.ConfigurationMessages.InvalidSiteUrl);
                Common.Helpers.MyLogger.Logger.Debug(uriException);
            }
            catch (WebException webException)
            {
                _generalUI.NotifyError(_notifyIcon, Common.Constants.ConfigurationMessages.BadConfigurationTitle, Common.Constants.ConfigurationMessages.CredentialsError);
                Common.Helpers.MyLogger.Logger.Debug(webException);
            }
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            _path = Common.Helpers.PathConfiguration.SetPath(_configuration.DirectoryPath);
            pathLabel.Content = _path;
        }
    }
}
