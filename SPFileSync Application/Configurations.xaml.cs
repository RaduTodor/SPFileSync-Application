
namespace SPFileSync_Application
{
    using Configuration;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    public partial class Configurations
    {
        private List<ConnectionConfiguration> _connections;
        private MainWindow mainWindow;
        // TODO:[CR BT]: If it's initialized on constructor remove this initialization.
        private ObservableCollection<string> _configurationsName = new ObservableCollection<string>();
        public Configurations(List<ConnectionConfiguration> connectionConfigurations, MainWindow window)
        {
            InitializeComponent();
            PopulateComboBox();
            _configurationsName = new ObservableCollection<string>();
            _connections = connectionConfigurations;
            PopulateObservableCollection();
            allConfigsList.ItemsSource = _configurationsName;
            mainWindow = window;         
        }
      
        private void PopulateComboBox()
        {
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxCsom);
        }

        private void PopulateObservableCollection()
        {
            _configurationsName = new ObservableCollection<string>();
            foreach (var item in _connections)
            {
                _configurationsName.Add(item.Connection.UriString);
            }
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            var selectedConfig = _connections[allConfigsList.SelectedIndex];
            EditConfigurationPanel window = new EditConfigurationPanel(selectedConfig, _connections, this);
            window.Show();
            this.Hide();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            _connections.RemoveAt(allConfigsList.SelectedIndex);
            _configurationsName.Remove(_configurationsName[allConfigsList.SelectedIndex]);
            Common.Helpers.XmlFileManipulator.Serialize(_connections);
            PopulateObservableCollection();
            allConfigsList.ItemsSource = _configurationsName;
            if (_connections.Count == 0)
            {
                mainWindow.SyncButton.IsEnabled = false;
            }
        }
       
    }
}
