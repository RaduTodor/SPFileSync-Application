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
    //TODO [CR BT] : Move usings inside the namespace.
    //TODO [CR BT] : Remove Window inheritance.
    public partial class MainWindow : Window
    {
        //TODO [CR BT] : Private proprties should be named starting with "_".
        List<ConnectionConfiguration> connectionConfigurations = new List<ConnectionConfiguration>();
        //TODO [CR BT] : Extract logic into multiple methods. eg. logic regarding NofityIcon should be extracted into another method.
        public MainWindow()
        {
            InitializeComponent();            
            configComboBox.Items.Add(ConfigurationMessages.comboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.comboBoxCsom);
            //TODO [CR BT] : Remove unused variable                  
            GeneralUI uIFunctions = new GeneralUI(this);
            //TODO [CR BT] : Rename ni variable
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

        //TODO [CR BT] : Please make this method private.
        //TODO [CR BT] : Use switch instead of multiple if. If there are 20 values for this Enum how you should proceed?
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
