using Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Forms;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for EditConfigurationPanel.xaml
    /// </summary>
    public partial class EditConfigurationPanel : Window
    {
        private ConnectionConfiguration configuration;
        private List<ConnectionConfiguration> configurations;
        private GeneralUI generalUI;
        private NotifyIcon notifyIcon = new NotifyIcon();       
        private string path="";
        public EditConfigurationPanel(ConnectionConfiguration configurationItem,List<ConnectionConfiguration> configurations)
        {
            InitializeComponent();
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.comboBoxRest);
            generalUI = new GeneralUI(this);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.comboBoxCsom);
            configuration = configurationItem;
            path = configuration.DirectoryPath;           
            this.configurations = configurations;
            siteUrlBox.Text = configuration.Connection.Uri.ToString();
            syncTextBox.Text = configuration.SyncTimeSpan.TotalMinutes.ToString();
            userNameTextBox.Text = configuration.Connection.Credentials.UserName;
            passwordText.Password = configuration.Connection.Credentials.Password;
        }

        private void ListEdit(object sender, RoutedEventArgs e)
        {
            // list logic
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                double minutes = 0;
                var checkSyncTime = double.TryParse(syncTextBox.Text, out minutes);
                if (checkSyncTime)
                {
                    Connection connection = new Connection() { Credentials = new Models.Credentials { UserName = userNameTextBox.Text, Password = passwordText.Password }, Uri = new Uri(siteUrlBox.Text) };
                    configuration.Connection = connection;
                    configuration.SyncTimeSpan = TimeSpan.FromMinutes(minutes);
                    configuration.Connection.Login();
                    configuration.DirectoryPath = path;
                    // edit list logic 
                    Common.Helpers.XmlFileManipulator.Serialize(configurations);
                    Close();
                }
                else
                {
                    generalUI.DisplayWarning(syncLabel, "Invalid value");
                }
            }
            catch(UriFormatException uriException)                          
            {
                generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.invalidSiteUrl);               
                Common.Helpers.MyLogger.Logger.Debug(uriException);
            }
            catch (WebException webException)
            {
                generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.credentialsError);
                Common.Helpers.MyLogger.Logger.Debug(webException);
            }
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = configuration.DirectoryPath;
            var result = folder.ShowDialog();
            path = folder.SelectedPath;

        }
    }
}
