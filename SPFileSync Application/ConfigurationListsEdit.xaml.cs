

namespace SPFileSync_Application
{
    using Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    public partial class ConfigurationListsEdit
    {
        List<ListWithColumnsName> _selectedConfigLists;
        //TODO [CR BT] Instantiate members from ctor
        private ObservableCollection<string> _observableSelectedconfigListsName = new ObservableCollection<string>();
        private List<ListWithColumnsName> _removedListsOfConfig = new List<ListWithColumnsName>();
        private int _selectedItemIndex;
        public ConfigurationListsEdit(List<ListWithColumnsName> list)
        {
            InitializeComponent();
            _selectedConfigLists = list;
            _observableSelectedconfigListsName.CollectionChanged += ItemsCollectionChanged;
            GetListsNames();
            itemListBox.ItemsSource = _observableSelectedconfigListsName;
        }

        private void GetListsNames()
        {
            foreach (var item in _selectedConfigLists)
            {
                _observableSelectedconfigListsName.Add(item.ListName);
            }
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {          
            if (_observableSelectedconfigListsName.Count <= 1)
            {
                removeButton.IsEnabled = false;
            }
            else
            {
                removeButton.IsEnabled = true;
            }
        }

        private void RemoveItemList(object sender, RoutedEventArgs e)
        {
            _selectedItemIndex = itemListBox.SelectedIndex;
            _observableSelectedconfigListsName.Remove((string)itemListBox.SelectedItem);
            _removedListsOfConfig.Add(_selectedConfigLists[_selectedItemIndex]);
        }

        private void EditItemList(object sender, RoutedEventArgs e)
        {
            _selectedItemIndex = itemListBox.SelectedIndex;            
            var selectedConfigList = _selectedConfigLists[_selectedItemIndex];
            EditItemListPanel window = new EditItemListPanel(selectedConfigList, this);
            window.Show();
            Hide();
        }

        private void RemoveLists(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _removedListsOfConfig.Count; i++)
            {
                _selectedConfigLists.Remove(_removedListsOfConfig[i]);
            }
            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            ListWithColumns window = new ListWithColumns(_selectedConfigLists, _observableSelectedconfigListsName);         
            window.Show();          
        }
    }
}
