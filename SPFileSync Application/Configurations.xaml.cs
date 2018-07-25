namespace SPFileSync_Application
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Common.Constants;
    using Common.Helpers;
    using Configuration;

    public partial class Configurations
    {
        private readonly ObservableCollection<string> _configurationsName = new ObservableCollection<string>();
        private readonly List<ConnectionConfiguration> _connections;
        private readonly MainWindow mainWindow;

        public Configurations(List<ConnectionConfiguration> connectionConfigurations, MainWindow window)
        {
            InitializeComponent();
            PopulateComboBox();
            _connections = connectionConfigurations;
            PopulateObservableCollection();
            allConfigsList.ItemsSource = _configurationsName;
            mainWindow = window;
        }

        private void PopulateComboBox()
        {
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxCsom);
        }

        private void PopulateObservableCollection()
        {
            foreach (var item in _connections) _configurationsName.Add(item.Connection.UriString);
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            var selectedConfig = _connections[allConfigsList.SelectedIndex];
            var window = new EditConfigurationPanel(selectedConfig, _connections, this);
            window.Show();
            Hide();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            _connections.RemoveAt(allConfigsList.SelectedIndex);
            _configurationsName.Remove((string) allConfigsList.SelectedItem);
            XmlFileManipulator.Serialize(_connections);
            if (_connections.Count == 0) mainWindow.SyncButton.IsEnabled = false;
        }
    }
}