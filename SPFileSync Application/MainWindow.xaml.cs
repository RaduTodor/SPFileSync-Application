using Configuration;
using System.Windows;
using System.Configuration;
using System.Collections.Generic;
using Models;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BusinessLogicLayer;
using Common.Constants;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        List<ConnectionConfiguration> connectionConfigurations = new List<ConnectionConfiguration>();
        public MainWindow()
        {
            InitializeComponent();            
            configComboBox.Items.Add(ConfigurationMessages.comboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.comboBoxCsom);                     
            GeneralUI uIFunctions = new GeneralUI(this);
            NotifyIcon ni = new NotifyIcon();
            var path = Directory.GetCurrentDirectory();
            var pathCombine = Path.Combine(path, "UFO.ico");
            ni.Icon = new Icon(pathCombine);
            ni.Visible = true;
            ni.Text = "SPFileSync";
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();          
        }
     
        private void AddConfig(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow window = new ConfigurationWindow(connectionConfigurations);
            window.Show();
        }
       
        private void Sync(object sender, RoutedEventArgs e)
        {
            FilesManager fileManager = new FilesManager(connectionConfigurations, GetProviderType(configComboBox.SelectedItem.ToString()));
            fileManager.Synchronize();
        }

        private void SeeConfigurations(object sender, RoutedEventArgs e)
        {           
            connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();
            Configurations window = new Configurations(connectionConfigurations);
            window.Show();          
        }

       public static Common.ApplicationEnums.ListReferenceProviderType GetProviderType(string choice)
        {
            if(choice == "Rest")
            {
                return Common.ApplicationEnums.ListReferenceProviderType.Rest;
            }
            else if(choice == "Csom")
            {
                return Common.ApplicationEnums.ListReferenceProviderType.Csom;
            }

            return Common.ApplicationEnums.ListReferenceProviderType.Rest;
        }
    }
}
