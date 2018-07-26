using Common.Helpers;
using Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusinessLogicLayer
{
    public class ConfigurationUIOperations
    {
        private Connection CreateConnection(ConfigurationWindowModel configurationWindowModel)
        {
            Connection connection = new Connection() { Credentials = new Credentials() { UserName = configurationWindowModel.UserName, Password = configurationWindowModel.Password },
                Uri = new Uri(configurationWindowModel.SiteUrl) };           
            return connection;
        }

        public bool EditConfiguration(ConfigurationWindowModel configurationWindowModel, List<ConnectionConfiguration> configurations, WindowNotifyModel windowNotifyModel,ConnectionConfiguration configuration)
        {
            try
            { 
                CreateBasicConfiguration(configurationWindowModel, configuration);
                
                SerializeConfigurations(configurations);
                return true;
            }
            catch (UriFormatException uriException)
            {
                windowNotifyModel.NotifyUI.CatchErrorNotifier(uriException,Common.Constants.ConfigurationMessages.InvalidSiteUrl);
                return false;
            }
            catch (Common.Exceptions.LoginException webException)
            {
                windowNotifyModel.NotifyUI.CatchErrorNotifier(webException,webException.Message);
                return false;
            }
            catch (Exception exception)
            {
                windowNotifyModel.NotifyUI.CatchErrorNotifier(exception,exception.Message);
                return false;
            }
        }
     
        private void CreateBasicConfiguration(ConfigurationWindowModel configurationWindowModel, ConnectionConfiguration connectionConfiguration)
        {
            var minutes = int.Parse(configurationWindowModel.SyncInterval);
            Connection connection = CreateConnection(configurationWindowModel);
            connection.Login();         
            Connection.CheckConfiguration(ref connectionConfiguration);
            connectionConfiguration.Connection = connection;
            connectionConfiguration.DirectoryPath = configurationWindowModel.Path;
            connectionConfiguration.SyncTimeSpan = TimeSpan.FromMinutes(minutes);             
        }

        public bool AddNewConfiguration(ConfigurationWindowModel configurationWindowModel, List<ConnectionConfiguration> configurations,WindowNotifyModel windowNotifyModel)
        {
            try
            {                    
                ConnectionConfiguration connectionConfiguration = new ConnectionConfiguration();
                CreateBasicConfiguration(configurationWindowModel, connectionConfiguration);
                ListWithColumnsName list = new ListWithColumnsName() { ListName = configurationWindowModel.ListName, UrlColumnName = configurationWindowModel.UrlColumn, UserColumnName = configurationWindowModel.UserColumn };
                if (connectionConfiguration.ListsWithColumnsNames == null)
                {
                    connectionConfiguration.ListsWithColumnsNames = new List<ListWithColumnsName>();
                }
                connectionConfiguration.ListsWithColumnsNames.Add(list);
                configurations.Add(connectionConfiguration);
                SerializeConfigurations(configurations);
                windowNotifyModel.Window.Close();
                return true;

            }
            catch (UriFormatException uriException)
            {
                windowNotifyModel.NotifyUI.CatchErrorNotifier(uriException,Common.Constants.ConfigurationMessages.InvalidSiteUrl);
                return false;
            }
            catch (Common.Exceptions.LoginException webException)
            {
                windowNotifyModel.NotifyUI.CatchErrorNotifier(webException,webException.Message);
                return false;
            }
            catch (Exception exception)
            {
                windowNotifyModel.NotifyUI.CatchErrorNotifier(exception,exception.Message);
                return false;
            }
        }

        private void SerializeConfigurations(List<ConnectionConfiguration> configurations)
        {
            XmlFileManipulator.Serialize(configurations);
        }
    }
}
