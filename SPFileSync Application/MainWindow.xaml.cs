using Configuration;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System;
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
            ni.Icon = new Icon(GeneralUI.GetResourcesFolder(ConfigurationMessages.ResourceFolderAppIcon));
            ni.Visible = true;
            ni.Text = ConfigurationMessages.AppName;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            connectionConfigurations = Common.Helpers.XmlFileManipulator.Deserialize<ConnectionConfiguration>();          
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            base.OnStateChanged(e);
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
            if(choice == ConfigurationMessages.comboBoxRest)
            {
                return Common.ApplicationEnums.ListReferenceProviderType.Rest;
            }
            else if(choice == ConfigurationMessages.comboBoxCsom)
            {
                return Common.ApplicationEnums.ListReferenceProviderType.Csom;
            }

            return Common.ApplicationEnums.ListReferenceProviderType.Rest;
        }
    }
}
