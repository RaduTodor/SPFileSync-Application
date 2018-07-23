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
    //TODO [CR BT] : Move usings inside the namespace.
    //TODO [CR BT] : Remove Window inheritance.
    public partial class EditConfigurationPanel : Window
    {
        //TODO [CR BT] : Private proprties should be named starting with "_".
        private ConnectionConfiguration configuration;
        private List<ConnectionConfiguration> configurations;
        private GeneralUI generalUI;
        private NotifyIcon notifyIcon = new NotifyIcon();       
        private string path="";
        Window window;

        //TODO [CR BT] : Extract logic in multiple methods. 
        public EditConfigurationPanel(ConnectionConfiguration configurationItem,List<ConnectionConfiguration> configurations,Window window)
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
            this.window = window;
            userNameTextBox.Text = configuration.Connection.Credentials.UserName;
            passwordText.Password = configuration.Connection.Credentials.Password;
        }

        private void ListEdit(object sender, RoutedEventArgs e)
        {
            ConfigurationListsEdit window = new ConfigurationListsEdit(configuration.ListsWithColumnsNames);
            window.Show();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            window.Show();
            Close();
        }
        //TODO [CR BT] : Create a model with all UI textboxes, send it to this method and move this logic into Business Layer. Create a class eg. ConfigurationValidator and add the method there.
        private bool ValidateAllFields()
        {
            bool input = false;
            var syncBoxValidation = generalUI.FieldValidation(syncTextBox.Text, syncLabel);   
            var userName = generalUI.FieldValidation(userNameTextBox.Text, userNameErrorLabel);
            var password = generalUI.FieldValidation(passwordText.Password, passwordErrorLabel);
            var siteUrl = generalUI.FieldValidation(siteUrlBox.Text, siteErrorLabel);                      
            if (syncBoxValidation && userName && password && siteUrl)
            {
                input = true;
            }
            return input;
        }

        //TODO [CR BT] : Duplicate code in catch. Extract class eg. NotifyUI and send corresponding parameters.
        //TODO [CR BT] : Extract try logic and move it into Business Logic. Use the UI textboxes model created above and send it to this class. 
        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                if(ValidateAllFields())
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
                        Common.Helpers.XmlFileManipulator.Serialize(configurations);
                        window.Show();
                        Close();
                    }
                    else
                    {
                        //TODO [CR BT] : Extract constant
                        generalUI.DisplayWarning(syncLabel, "Invalid value");
                    }
                }                              
            }
            catch(UriFormatException uriException)                          
            {
                generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.invalidSiteUrl);               
                Common.Helpers.MyLogger.Logger.Debug(uriException,"error");
            }
            catch (WebException webException)
            {
                generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.credentialsError);
                Common.Helpers.MyLogger.Logger.Debug(webException);
            }
        }
        //TODO [CR BT] : Remove unused variable "result".
        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = configuration.DirectoryPath;           
            var result = folder.ShowDialog();
            path = folder.SelectedPath;

        }
    }
}
