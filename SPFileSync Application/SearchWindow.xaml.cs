using BusinessLogicLayer;
using Common.ApplicationEnums;
using Common.Constants;
using Common.Helpers;
using Configuration;
using Models;
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
//TODO CR: Remove unused using and move the rest inside the namespace
namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private List<ConnectionConfiguration> _configurations;
        private ObservableHashSet<string> _configurationsURL;
        private FileSearcher _fileSearcher;
        private NotifyUI _notifyUI = new NotifyUI();
        private Dictionary<string, string> _searchedElements;
        private ObservableCollection<string> _listsName;
        private ObservableCollection<string> _dictionaryValues;
        private List<string> _dictionaryKeys;
        private List<ListWithColumnsName> _lists;
        public SearchWindow(List<ConnectionConfiguration> configurations)
        {
            InitializeComponent();
            _configurations = configurations;
            _configurationsURL = new ObservableHashSet<string>();
            _searchedElements = new Dictionary<string, string>();
            GetConfigurationsURL();
            siteComboBox.SelectionChanged += SearchedItemsSelectionChanged;
            PopulateUIComboBox();
            siteComboBox.ItemsSource = _configurationsURL;
        }

        private void SearchedItemsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewConfigSelected();
        }

        private void PopulateUIComboBox()
        {
            providerTypeComboBox.Items.Add(ConfigurationMessages.ComboBoxRest);
            providerTypeComboBox.Items.Add(ConfigurationMessages.ComboBoxCsom);
        }

        private void GetConfigurationsURL()
        {
            foreach (var configuration in _configurations)
            {
                _configurationsURL.Add(configuration.Connection.UriString);
            }
        }

        private void PopulateListsList()
        {
            _listsName = new ObservableCollection<string>();
            foreach (var item in _lists) _listsName.Add(item.ListName);
        }

        private void NewConfigSelected()
        {
            _listsName = new ObservableCollection<string>();
            referenceList.ItemsSource = _listsName;     
            _lists = _configurations[siteComboBox.SelectedIndex].ListsWithColumnsNames;
            PopulateListsList();
            referenceList.UnselectAll();          
            referenceList.ItemsSource = _listsName;
        }

        private void AddToReferenceList(object sender, RoutedEventArgs e)
        {
            var itemUrl = _dictionaryKeys[searchedItems.SelectedIndex];
            var listReferenceManager = CreateListReferenceProvider();
            listReferenceManager.AddSyncListItem(_lists[referenceList.SelectedIndex].ListName,
                itemUrl);
        }

        private void PopulateListElements()
        {
            _dictionaryValues = new ObservableCollection<string>();
            _dictionaryKeys = new List<string>();
            foreach (var item in _searchedElements)
            {
                _dictionaryValues.Add(item.Value);
                _dictionaryKeys.Add(item.Key);
            }
        }

        private void PopulateDictionary()
        {
            ConnectionConfiguration configuration = _configurations.FirstOrDefault(connection => connection.Connection.UriString == siteComboBox.SelectedItem.ToString());
            _fileSearcher = new FileSearcher(configuration, GetProviderType(providerTypeComboBox.SelectedItem.ToString()));
            _searchedElements = _fileSearcher.SearchSpFiles(searchField.Text).Result;         
        }

        private void SearchItem(object sender, RoutedEventArgs e)
        {
            searchedItems.ItemsSource = null;
            searchedItems.UnselectAll();
            PopulateDictionary();
            PopulateListElements();
            searchedItems.ItemsSource = _dictionaryValues;
        }

        private static ListReferenceProviderType GetProviderType(string choice)
        {
            switch (choice)
            {
                case ConfigurationMessages.ComboBoxRest:
                    return ListReferenceProviderType.Rest;

                case ConfigurationMessages.ComboBoxCsom:
                    return ListReferenceProviderType.Csom;

                default:
                    return ListReferenceProviderType.Rest;
            }
        }

        private ListReferenceManager CreateListReferenceProvider()
        {
            ConnectionConfiguration configuration = _configurations.FirstOrDefault(connection => connection.Connection.UriString == siteComboBox.SelectedItem.ToString());
            ListReferenceManager listReferenceManager;
            if (providerTypeComboBox.SelectionBoxItem.ToString() == ConfigurationMessages.ComboBoxRest)
                listReferenceManager = new ListReferenceManager(configuration, ListReferenceProviderType.Rest);
            else
                listReferenceManager = new ListReferenceManager(configuration, ListReferenceProviderType.Csom);
            return listReferenceManager;
        }
    }
}
