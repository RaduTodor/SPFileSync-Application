namespace SPFileSync_Application
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Models;

    public partial class ListWithColumns
    {
        private readonly List<ListWithColumnsName> _addListsToConfiguration;
        /// TODO:[CR BT]: Add _ to configurationListsName
        private readonly ObservableCollection<string> configurationListsName;

        public ListWithColumns(List<ListWithColumnsName> list)
        {
            InitializeComponent();
            _addListsToConfiguration = list;
            configurationListsName = new ObservableCollection<string>();
            configurationListsName = new ObservableCollection<string>();
        }

        public ListWithColumns(List<ListWithColumnsName> list, ObservableCollection<string> observableCollection)
        {
            InitializeComponent();
            _addListsToConfiguration = list;
            configurationListsName = observableCollection;
        }

        private void ConfirmList(object sender, RoutedEventArgs e)
        {
            var list = new ListWithColumnsName
            {
                ListName = listTextBox.Text,
                UrlColumnName = urlColumnTextBox.Text,
                UserColumnName = userColumnTextBox.Text
            };
            _addListsToConfiguration.Add(list);
            configurationListsName.Add(list.ListName);
            Close();
        }

        private void CancelList(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}