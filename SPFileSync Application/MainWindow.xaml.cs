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
            ConnectionConfiguration firstConfiguration=ButtonActions.AddConnectionButtonPressed(ConfigurationManager.AppSettings["SharePointURL"], 
                ConfigurationManager.AppSettings["Account"], 
                ConfigurationManager.AppSettings["Password"]);
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
                ConfigurationManager.AppSettings["Password"]);
            listsWithColumnsNames = new List<ListWithColumnsName>();
            listsWithColumnsNames.Add(new ListWithColumnsName
            {
                ListName = "SyncList",
                UrlColumnName = "URL",
                UserColumnName = "Utilizator"
            });

            secondConfiguration.ListsWithColumnsNames = listsWithColumnsNames;
            connectionConfigurations.Add(secondConfiguration);

            ButtonActions.SynchronizeButtonPressed(connectionConfigurations);
        }
    }
}
