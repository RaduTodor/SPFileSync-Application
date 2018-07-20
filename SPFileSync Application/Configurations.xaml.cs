using Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for Configurations.xaml
    /// </summary>
    public partial class Configurations : Window
    {
        private List<ConnectionConfiguration> connections;
        private ObservableCollection<ConnectionConfiguration> items;
        public Configurations(List<ConnectionConfiguration> connectionConfigurations)
        {
            InitializeComponent();
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.comboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.comboBoxCsom);
            connections = connectionConfigurations;
            items = new ObservableCollection<ConnectionConfiguration>(connections);
            allConfigsList.ItemsSource = items;                      
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            EditConfigurationPanel window = new EditConfigurationPanel((ConnectionConfiguration)allConfigsList.SelectedItem,connections);
            window.Show();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            ConnectionConfiguration selectedConfig = (ConnectionConfiguration)allConfigsList.SelectedItem;
            items.Remove(selectedConfig); 
            connections.Remove(selectedConfig);
            Common.Helpers.XmlFileManipulator.Serialize(connections);
        }
    }
}
