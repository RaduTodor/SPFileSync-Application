
namespace SPFileSync_Application
{
    using BusinessLogicLayer;
    using Common.Helpers;
    using Configuration;
    using Models;
    using System.Collections.Generic;
    using System.Windows;

    public partial class ConfigurationWindow
    {

        private string _path;       
        private ConnectionConfiguration _configuration;
        private NotifyUI _notifyUI;
        private List<ConnectionConfiguration> _configurations;
        private MainWindow mainWindow;
        public ConfigurationWindow(List<ConnectionConfiguration> connectionConfigurations, MainWindow window)
        {
            InitializeComponent();
            _notifyUI = new NotifyUI(this,errorList);
            _configurations = connectionConfigurations;
            mainWindow = window;
            InitializeUI();
            mainWindow = window;
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
            WindowNotifyModel windowNotifyModel = new WindowNotifyModel() {NotifyUi = _notifyUI, Window = this};
            var test =configurationOperations.AddNewConfiguration(configurationWindowModel, _configurations,windowNotifyModel);
                if (test && mainWindow.SyncButton.IsEnabled == false)
                    mainWindow.SyncButton.IsEnabled = true;
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
