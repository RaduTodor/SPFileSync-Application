using Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Forms;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    //TODO [CR BT] : Move usings inside the namespace.
    //TODO [CR BT] : Remove Window inheritance.
    public partial class ConfigurationWindow : Window
    {

        //Should i move the buttons logic in methods and move them into Business Logic Layer?
        //TODO [CR BT] : Private proprties should be named starting with "_".
        private string path;
        private NotifyIcon notifyIcon = new NotifyIcon();
        private ConnectionConfiguration configuration;
        private GeneralUI generalUI;
        private List<ConnectionConfiguration> configurations;
        public ConfigurationWindow(List<ConnectionConfiguration> connectionConfigurations)
        {
            InitializeComponent();
            generalUI = new GeneralUI(this);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.comboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.comboBoxCsom);
            configurations = connectionConfigurations;
        }

        private void SetFileDestination(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            var result = folder.ShowDialog();
            path = folder.SelectedPath;
        }

        //TODO [CR BT] : Create a model with all UI textboxes, send it to this method and move this logic into Business Layer. Create a class eg. ConfigurationValidator and add the method there.
        private bool ValidateAllFields()
        {
            bool input = false;
            var syncBoxValidation = generalUI.FieldValidation(syncTextBox.Text, syncLabel);
            var pathValidation = generalUI.FieldValidation(path, pathLabel);
            var userName = generalUI.FieldValidation(userNameTextBox.Text, userNameErrorLabel);
            var password = generalUI.FieldValidation(passwordText.Password, passwordErrorLabel);
            var siteUrl = generalUI.FieldValidation(siteUrlBox.Text, siteErrorLabel);
            var listUrl = generalUI.FieldValidation(listTextBox.Text, listErrorLabel);
            var urlColumn = generalUI.FieldValidation(urlColumnTextBox.Text, urlColumnError);
            var userColumn = generalUI.FieldValidation(userColumnTextBox.Text, userColumnError);
            if (syncBoxValidation && pathValidation && userName && password && siteUrl && listUrl && urlColumn && userColumn)
            {
                input = true;
            }
            return input;
        }

        //TODO [CR BT] : Duplicate code in catch. Extract class eg. NotifyUI and send corresponding parameters.
        //TODO [CR BT] : Extract try logic and move it into Business Logic. Use the UI textboxes model created above and send it to this class. Split this logic in multiple methods. This logic is making two different operations: create a connection and add a list to configuration which will be serialized .
        private void Save(object sender, RoutedEventArgs e)
        {
            if (ValidateAllFields())
            {
                try
                {
                    Connection connection = new Connection() { Credentials = new Credentials() { UserName = userNameTextBox.Text, Password = passwordText.Password }, Uri = new Uri(siteUrlBox.Text) };
                    var minutes = int.Parse(syncTextBox.Text);
                    connection.Login();
                    GeneralUI.checkConfiguration(configuration);
                    configuration.Connection = connection;
                    configuration.DirectoryPath = path;
                    configuration.SyncTimeSpan = TimeSpan.FromMinutes(minutes);                      
                    ListWithColumnsName list =  new ListWithColumnsName() { ListName = listTextBox.Text, UrlColumnName = urlColumnTextBox.Text, UserColumnName = userColumnTextBox.Text };
                    if(configuration.ListsWithColumnsNames == null)
                    {
                        configuration.ListsWithColumnsNames = new List<ListWithColumnsName>();
                    }
                    configuration.ListsWithColumnsNames.Add(list);
                    configurations.Add(configuration);                    
                    Common.Helpers.XmlFileManipulator.Serialize(configurations);
                    this.Close();
                }
                catch(UriFormatException uriException)
                {
                    generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.invalidSiteUrl);
                    generalUI.AddToListButton(Common.Constants.ConfigurationMessages.invalidSiteUrl);
                    Common.Helpers.MyLogger.Logger.Debug(uriException);
                }
                catch (WebException webException)
                {
                    generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.credentialsError);
                    generalUI.AddToListButton(Common.Constants.ConfigurationMessages.credentialsError);
                    Common.Helpers.MyLogger.Logger.Debug(webException);
                }
                catch (Exception exception)
                {
                    generalUI.NotifyError(notifyIcon, Common.Constants.ConfigurationMessages.badConfigurationTitle, Common.Constants.ConfigurationMessages.generalConfigError);
                    generalUI.AddToListButton(Common.Constants.ConfigurationMessages.generalConfigError);
                    Common.Helpers.MyLogger.Logger.Debug(exception);
                }               
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {            
            this.Close();           
        }
      
        private void AddListWithColumns(object sender, RoutedEventArgs e)
        {

            GeneralUI.checkConfiguration(configuration);
            ListWithColumns window = new ListWithColumns(configuration.ListsWithColumnsNames);
            window.Show();
        }        
    }
}
