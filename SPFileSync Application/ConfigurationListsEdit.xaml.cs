namespace SPFileSync_Application
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows;
    using Models;

    public partial class ConfigurationListsEdit
    {
        private readonly ObservableCollection<string> _observableSelectedconfigListsName =
            new ObservableCollection<string>();

        private readonly List<ListWithColumnsName> _removedListsOfConfig = new List<ListWithColumnsName>();
        private readonly List<ListWithColumnsName> _selectedConfigLists;

        private int _selectedItemIndex;

        //TODO [CR BT] : This seems and should to be a constant.
        private int countRemovedSelectedListItems;

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
            foreach (var item in _selectedConfigLists) _observableSelectedconfigListsName.Add(item.ListName);
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var action = e.Action.ToString();
            if (action == "Remove") countRemovedSelectedListItems++;

            if (_observableSelectedconfigListsName.Count <= 1)
                removeButton.IsEnabled = false;
            else
                removeButton.IsEnabled = true;
        }

        private void RemoveItemList(object sender, RoutedEventArgs e)
        {
            _selectedItemIndex = itemListBox.SelectedIndex;
            _observableSelectedconfigListsName.Remove((string) itemListBox.SelectedItem);
            _removedListsOfConfig.Add(_selectedConfigLists[_selectedItemIndex]);
        }

        private void EditItemList(object sender, RoutedEventArgs e)
        {
            _selectedItemIndex = itemListBox.SelectedIndex;
            var selectedConfigList = _selectedConfigLists[_selectedItemIndex];
            var window = new EditItemListPanel(selectedConfigList, this);
            window.Show();
            Hide();
        }

        private void RemoveLists(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < _removedListsOfConfig.Count; i++) _selectedConfigLists.Remove(_removedListsOfConfig[i]);
            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            var window = new ListWithColumns(_selectedConfigLists, _observableSelectedconfigListsName);
            window.Show();
        }
    }
}