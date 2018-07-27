namespace SPFileSync_Application
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using BusinessLogicLayer;
    using Common.ApplicationEnums;
    using Common.Constants;
    using Configuration;
    using DataAccessLayer;
    using Models;

    /// <summary>
    ///     Interaction logic for ReferenceListOperationsWindow.xaml
    /// </summary>
    public partial class ReferenceListOperationsWindow : Window
    {
        private const string DefaultUrlBoxMessage = "Insert New URL";

        private const int NullChoiceValue = -1;
        private readonly List<ConnectionConfiguration> _connections;

        private ObservableCollection<string> _configurationsName;

        private List<ListWithColumnsName> _lists;

        private ObservableCollection<string> _listsName;

        private List<UrlListItem> _urls;

        private List<int> _urlsId;

        private ObservableCollection<string> _urlsNames;

        //TODO [CR RT]: Extract logic in multiple methods. This method it's making almost 6 different things.
        public ReferenceListOperationsWindow(List<ConnectionConfiguration> connectionConfigurations)
        {
            InitializeComponent();
            PopulateComboBox();
            RemoveButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            AddButton.IsEnabled = false;

            _configurationsName = new ObservableCollection<string>();
            _listsName = new ObservableCollection<string>();
            _urlsNames = new ObservableCollection<string>();

            NewUrlTextBox.TextChanged += (sender, args) =>
            {
                if (Uri.IsWellFormedUriString(NewUrlTextBox.Text, UriKind.Absolute))
                {
                    AddButton.IsEnabled = true;
                    if (allUrlsList.SelectedIndex != NullChoiceValue)
                    {
                        EditButton.IsEnabled = true;
                    }
                }
                else
                {
                    AddButton.IsEnabled = false;
                }
            };

            _connections = connectionConfigurations;
            PopulateConfigurationsObservableCollection();
            allConfigsList.ItemsSource = _configurationsName;

            allConfigsList.SelectionChanged += (sender, args) => { NewConfigSelected(); };
            allConfigListsList.SelectionChanged += (sender, args) => { NewListSelected(); };
            allUrlsList.SelectionChanged += (sender, args) => { NewUrlSelected(); };
        }

        private void NewConfigSelected()
        {
            _listsName = new ObservableCollection<string>();
            allConfigListsList.ItemsSource = _listsName;
            _urlsNames = new ObservableCollection<string>();
            allUrlsList.ItemsSource = _urlsNames;
            _lists = _connections[allConfigsList.SelectedIndex].ListsWithColumnsNames;
            PopulateListsObservableCollection();
            allConfigListsList.UnselectAll();
            allUrlsList.UnselectAll();
            allConfigListsList.ItemsSource = _listsName;
        }

        private void NewUrlSelected()
        {
            if (allUrlsList.SelectedIndex != NullChoiceValue)
            {
                RemoveButton.IsEnabled = true;
                if (Uri.IsWellFormedUriString(NewUrlTextBox.Text, UriKind.Absolute))
                {
                    EditButton.IsEnabled = true;
                }
                else
                {
                    EditButton.IsEnabled = false;
                }
            }
            else
            {
                RemoveButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }

        private void NewListSelected()
        {
            if (allConfigListsList.SelectedIndex != NullChoiceValue)
            {
                _urlsNames = new ObservableCollection<string>();
                allUrlsList.ItemsSource = _urlsNames;
                var metadataProvider = new MetadataProvider(_connections[allConfigsList.SelectedIndex]);
                _urls = metadataProvider.GetCurrentUserUrlsFromList(_lists[allConfigListsList.SelectedIndex], null,
                    null);
                PopulateUrlsObservableCollection();
                allUrlsList.ItemsSource = _urlsNames;
                if (Uri.IsWellFormedUriString(NewUrlTextBox.Text, UriKind.Absolute))
                {
                    AddButton.IsEnabled = true;
                }
            }
            else
            {
                AddButton.IsEnabled = false;
            }
        }

        private void PopulateComboBox()
        {
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxRest);
            configComboBox.Items.Add(ConfigurationMessages.ComboBoxCsom);
        }

        private void PopulateConfigurationsObservableCollection()
        {
            _configurationsName = new ObservableCollection<string>();
            foreach (var item in _connections) _configurationsName.Add(item.Connection.UriString);
        }

        private void PopulateListsObservableCollection()
        {
            _listsName = new ObservableCollection<string>();
            foreach (var item in _lists) _listsName.Add(item.ListName);
        }

        //TODO [CR RT]: Please rename methong. Do not use names of lists here like Observable collection. eg. PopulateUrlsList
        private void PopulateUrlsObservableCollection()
        {
            _urlsNames = new ObservableCollection<string>();
            _urlsId = new List<int>();
            foreach (var item in _urls)
            {
                _urlsNames.Add(item.Url);
                _urlsId.Add(item.Id);
            }
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        { //TODO [CR RT]: Move the logic back here. You extracted all the logic from here only to put in on another method and this method have just one line. This have sense only if the method OperationButtonAction it's called in another location.
            OperationButtonAction(sender);
        }

        private void OperationButtonAction(object sender)
        {
            var listReferenceProvider = GetNewListProvider();
            if (Equals(sender, AddButton))
            {
                listReferenceProvider.AddListReferenceItem(_lists[allConfigListsList.SelectedIndex].ListName,
                    new Uri(NewUrlTextBox.Text));
                NewUrlTextBox.Text = DefaultUrlBoxMessage;
            }
            else if (Equals(sender, EditButton))
            {
                listReferenceProvider.ChangeListReferenceItem(new Uri(NewUrlTextBox.Text),
                    _urlsId[allUrlsList.SelectedIndex], _lists[allConfigListsList.SelectedIndex].ListName);
                NewUrlTextBox.Text = DefaultUrlBoxMessage;
            }
            else if (Equals(sender, RemoveButton))
            {
                listReferenceProvider.RemoveListReferenceItem(_lists[allConfigListsList.SelectedIndex].ListName,
                    _urlsId[allUrlsList.SelectedIndex]);
                RemoveButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }

            NewListSelected();
        }
        //TODO [CR RT]: Please rename the method to a more intuitive name. eg GetListReferenceProvide. What means "new".
        private BaseListReferenceProvider GetNewListProvider()
        {
            BaseListReferenceProvider listReferenceProvider;
            if (configComboBox.SelectionBoxItem.ToString() == ConfigurationMessages.ComboBoxRest)
                listReferenceProvider = OperationsFactory.GetOperations(ListReferenceProviderType.Rest);
            else
                listReferenceProvider = OperationsFactory.GetOperations(ListReferenceProviderType.Csom);

            listReferenceProvider.ConnectionConfiguration = _connections[allConfigsList.SelectedIndex];
            return listReferenceProvider;
        }
    }
}