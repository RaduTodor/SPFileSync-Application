using Configuration;
using System.Windows;
using System.Configuration;
using System.Collections.Generic;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.SharePoint.Client;
using Models;
using System;

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
            Connection connection = new Connection(new System.Uri(ConfigurationManager.AppSettings["SharePointURL"]), 
                new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Account"], ConfigurationManager.AppSettings["Password"]));
            Dictionary<string, List<string>> listsAndColumns = new Dictionary<string, List<string>>();
            List<string> columns = new List<string>();
            columns.Add("URL");
            columns.Add("User");
            listsAndColumns.Add("SyncList", columns);
            ConnectionConfiguration connectionConfiguration = new ConnectionConfiguration
            {
                Connection = connection,
                ListsAndColumns = listsAndColumns,
                DirectoryPath = ConfigurationManager.AppSettings["DirectoryPath"],
                SyncTimeSpan = new System.TimeSpan()
            };
            DataAccessOperations dao = new DataAccessOperations(connectionConfiguration, "SyncList");
            //ListOperations.DownloadFilesOfUser(dao);
            FileSynchronizer fileSynchronizer = new FileSynchronizer { DataAccessOperations = dao };
            fileSynchronizer.Synchronize();
        }
    }
}
