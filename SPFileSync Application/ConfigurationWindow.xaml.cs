
namespace SPFileSync_Application
{
    using Configuration;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows;
    using System.Windows.Forms;

    public partial class ConfigurationWindow
    {
        private MainWindow mainWindow;
        private string _path;
        private NotifyIcon _notifyIcon = new NotifyIcon();
        private ConnectionConfiguration _configuration;
        private GeneralUI _generalUI;
        private List<ConnectionConfiguration> _configurations;
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
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxCsom);         
        }
     
        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            _path = Common.Helpers.PathConfiguration.SetPath();
        }
           
        //TODO [CR BT] : Duplicate code in catch. Extract class eg. NotifyUI and send corresponding parameters.
        //TODO [CR BT] : Extract try logic and move it into Business Logic. Use the UI textboxes model created above and send it to this class. Split this logic in multiple methods. This logic is making two different operations: create a connection and add a list to configuration which will be serialized .
        private void Save(object sender, RoutedEventArgs e)
        {          
                try
                {
                    Connection connection = new Connection() { Credentials = new Credentials() { UserName = userNameTextBox.Text, Password = passwordText.Password }, Uri = new Uri(siteUrlBox.Text) };
                    var minutes = int.Parse(syncTextBox.Text);
                    connection.Login();
                    GeneralUI.checkConfiguration(ref _configuration);
                    _configuration.Connection = connection;
                    _configuration.DirectoryPath = _path;
                    _configuration.SyncTimeSpan = TimeSpan.FromMinutes(minutes);
                    ListWithColumnsName list = new ListWithColumnsName() { ListName = listTextBox.Text, UrlColumnName = urlColumnTextBox.Text, UserColumnName = userColumnTextBox.Text };
                    if (_configuration.ListsWithColumnsNames == null)
                    {
                        _configuration.ListsWithColumnsNames = new List<ListWithColumnsName>();
                    }
                    _configuration.ListsWithColumnsNames.Add(list);
                    _configurations.Add(_configuration);
                    Common.Helpers.XmlFileManipulator.Serialize(_configurations);
                    if (mainWindow.SyncButton.IsEnabled == false)
                        mainWindow.SyncButton.IsEnabled = true;
                this.Close();
                }
                catch (UriFormatException uriException)
                {
                    _generalUI.NotifyError(_notifyIcon, Common.Constants.ConfigurationMessages.BadConfigurationTitle, Common.Constants.ConfigurationMessages.InvalidSiteUrl);
                    _generalUI.AddToListButton(Common.Constants.ConfigurationMessages.InvalidSiteUrl);
                    Common.Helpers.MyLogger.Logger.Debug(uriException);
                }
                catch (Common.Exceptions.LoginException webException)
                {
                    _generalUI.NotifyError(_notifyIcon, Common.Constants.ConfigurationMessages.BadConfigurationTitle, Common.Constants.ConfigurationMessages.CredentialsError);
                    _generalUI.AddToListButton(Common.Constants.ConfigurationMessages.CredentialsError);

                    Common.Helpers.MyLogger.Logger.Debug(webException);
                }
                catch (Exception exception)
                {
                    _generalUI.NotifyError(_notifyIcon, Common.Constants.ConfigurationMessages.BadConfigurationTitle, Common.Constants.ConfigurationMessages.GeneralConfigError);
                    _generalUI.AddToListButton(Common.Constants.ConfigurationMessages.GeneralConfigError);
                    Common.Helpers.MyLogger.Logger.Debug(exception);
                }            
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddListWithColumns(object sender, RoutedEventArgs e)
        {

            GeneralUI.checkConfiguration(ref _configuration);
            ListWithColumns window = new ListWithColumns(_configuration.ListsWithColumnsNames);
            window.Show();
        }
    }
}
