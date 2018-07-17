using Configuration;
using System.Windows;
using System.Configuration;
using System.Collections.Generic;
using Models;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<ConnectionConfiguration> connectionConfigurations = new List<ConnectionConfiguration>();
            ConnectionConfiguration firstConfiguration = ButtonActions.AddConnectionButtonPressed(ConfigurationManager.AppSettings["SharePointURL"],
                ConfigurationManager.AppSettings["Account"],
                ConfigurationManager.AppSettings["Password"], 1);
            List<ListWithColumnsName> listsWithColumnsNames = new List<ListWithColumnsName>();
            listsWithColumnsNames.Add(new ListWithColumnsName
            {
                ListName = "SyncList",
                UrlColumnName = "URL",
                UserColumnName = "User"
            });

            firstConfiguration.ListsWithColumnsNames = listsWithColumnsNames;
            connectionConfigurations.Add(firstConfiguration);

            ConnectionConfiguration secondConfiguration = ButtonActions.AddConnectionButtonPressed(ConfigurationManager.AppSettings["SecondSharePointURL"],
                ConfigurationManager.AppSettings["Account"],
                ConfigurationManager.AppSettings["Password"], 2);
            listsWithColumnsNames = new List<ListWithColumnsName>();
            listsWithColumnsNames.Add(new ListWithColumnsName
            {
                ListName = "SyncList",
                UrlColumnName = "URL",
                UserColumnName = "Utilizator"
            });

            secondConfiguration.ListsWithColumnsNames = listsWithColumnsNames;
            connectionConfigurations.Add(secondConfiguration);

            ButtonActions.SynchronizeButtonPressed(connectionConfigurations, 2);

            //ButtonActions.AddSyncListItem(firstConfiguration, "http://binebine", "SyncList", 1);
            //ButtonActions.AddSyncListItem(firstConfiguration, "http://binebine", "SyncList", 2);
            //ButtonActions.ChangeSyncListItem(firstConfiguration, "http://simaibine", 28, "SyncList", 1);
            //ButtonActions.ChangeSyncListItem(firstConfiguration, "http://simaibine", 29, "SyncList", 2);
            //ButtonActions.RemoveSyncListItem(firstConfiguration, 28, "SyncList", 2);
            //ButtonActions.RemoveSyncListItem(firstConfiguration, 29, "SyncList", 1);
            //ButtonActions.RemoveSyncListItem(firstConfiguration, 31, "SyncList", 2);
            //ButtonActions.RemoveSyncListItem(firstConfiguration, 30, "SyncList", 1);

        }
    }
}
