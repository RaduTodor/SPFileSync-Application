﻿
namespace SPFileSync_Application
{
    using Configuration;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    public partial class Configurations
    {
        private List<ConnectionConfiguration> _connections;
        private ObservableCollection<string> _configurationsName = new ObservableCollection<string>();
        public Configurations(List<ConnectionConfiguration> connectionConfigurations)
        {
            InitializeComponent();
            PopulateComboBox();
            _connections = connectionConfigurations;
            PopulateObservableCollection();
            allConfigsList.ItemsSource = _configurationsName;
        }

        private void PopulateComboBox()
        {
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(Common.Constants.ConfigurationMessages.ComboBoxCsom);
        }

        private void PopulateObservableCollection()
        {
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
            //  ConnectionConfiguration selectedConfig = (ConnectionConfiguration)allConfigsList.SelectedItem;
            _configurationsName.Remove((string)allConfigsList.SelectedItem);
            //_connections.Remove(selectedConfig);
            _connections.RemoveAt(allConfigsList.SelectedIndex);
            Common.Helpers.XmlFileManipulator.Serialize(_connections);
        }
    }
}
