using Configuration;
using System.Windows;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer;
using Common.Constants;
using Models;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //TODO [CR RT]: Extract constants
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var hardcodedConfigs = InitializeConfigurationList();

            var connectionConfigurations = ConnectionConfigurationXmlManipulator.Deserialize();
            FilesManager filesManager = new FilesManager(connectionConfigurations, ApplicationEnums.ListReferenceProviderType.REST);
            filesManager.Synchronize();

            //ConnectionConfigurationXmlManipulator.Serialize(connectionConfigurations);
        }

        public List<ConnectionConfiguration> InitializeConfigurationList()
        {
            var connectionConfigurations = new List<ConnectionConfiguration>();
            var firstConfiguration = ButtonActions.AddConnectionButtonPressed(ConfigurationManager.AppSettings["SharePointURL"],
                ConfigurationManager.AppSettings["Account"],
                ConfigurationManager.AppSettings["Password"], 1);
            var listsWithColumnsNames = new List<ListWithColumnsName>();
            listsWithColumnsNames.Add(new ListWithColumnsName
            {
                ListName = "SyncList",
                UrlColumnName = "URL",
                UserColumnName = "User"
            });

            firstConfiguration.DirectoryPath = $"{ConfigurationManager.AppSettings[("DirectoryPath")]}\\{connectionConfigurations.Count}";
            firstConfiguration.SyncTimeSpan = new System.TimeSpan(0, 15, 0);
            firstConfiguration.ListsWithColumnsNames = listsWithColumnsNames;
            connectionConfigurations.Add(firstConfiguration);

            var secondConfiguration = ButtonActions.AddConnectionButtonPressed(ConfigurationManager.AppSettings["SecondSharePointURL"],
                ConfigurationManager.AppSettings["Account"],
                ConfigurationManager.AppSettings["Password"], 2);
            listsWithColumnsNames = new List<ListWithColumnsName>();
            listsWithColumnsNames.Add(new ListWithColumnsName
            {
                ListName = "SyncList",
                UrlColumnName = "URL",
                UserColumnName = "Utilizator"
            });
            secondConfiguration.DirectoryPath = $"{ConfigurationManager.AppSettings[("DirectoryPath")]}\\{connectionConfigurations.Count}";
            secondConfiguration.SyncTimeSpan = new System.TimeSpan(0, 10, 0);
            secondConfiguration.ListsWithColumnsNames = listsWithColumnsNames;
            connectionConfigurations.Add(secondConfiguration);
            return connectionConfigurations;
        }
    }
}
