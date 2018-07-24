

namespace SPFileSync_Application
{
    using Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    public partial class ListWithColumns 
    {
        private List<ListWithColumnsName> _addListsToConfiguration;
        private ObservableCollection<string> _meinObservableCollection;
        public ListWithColumns(List<ListWithColumnsName> list)
        {
            InitializeComponent();
            _addListsToConfiguration = list;   
            _meinObservableCollection = new ObservableCollection<string>();
        }

        public ListWithColumns(List<ListWithColumnsName> list,ObservableCollection<string> observableCollection)
        {
            InitializeComponent();
            _addListsToConfiguration = list;
            _meinObservableCollection = observableCollection;
        }

        private void ConfirmList(object sender, RoutedEventArgs e)
        {
            ListWithColumnsName list = new ListWithColumnsName() { ListName = listTextBox.Text, UrlColumnName = urlColumnTextBox.Text, UserColumnName = userColumnTextBox.Text };
            _addListsToConfiguration.Add(list);
            _meinObservableCollection.Add(list.ListName);
            this.Close();
        }

        private void CancelList(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
