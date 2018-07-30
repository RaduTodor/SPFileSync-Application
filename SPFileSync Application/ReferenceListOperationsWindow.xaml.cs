namespace SPFileSync_Application
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
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

        public ReferenceListOperationsWindow(List<ConnectionConfiguration> connectionConfigurations)
        {
            InitializeComponent();
            PopulateComboBox();
            
            InstantiateFields();

            AddModifyHandlerToTextBox(NewUrlTextBox);

            _connections = connectionConfigurations;
            PopulateConfigsList();
            allConfigsList.ItemsSource = _configurationsName;

            AddSelectionHandlersToListViews();
        }

        public void AddModifyHandlerToTextBox(TextBox textBox)
        {
            textBox.TextChanged += (sender, args) =>
            {
                if (Uri.IsWellFormedUriString(textBox.Text, UriKind.Absolute))
                {
                    if (allConfigListsList.SelectedIndex != NullChoiceValue)
                    {
                        AddButton.IsEnabled = true;
                    }

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
        }

        private void InstantiateFields()
        {
            RemoveButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            AddButton.IsEnabled = false;

            _configurationsName = new ObservableCollection<string>();
            _listsName = new ObservableCollection<string>();
            _urlsNames = new ObservableCollection<string>();
        }

        private void AddSelectionHandlersToListViews()
        {
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
            PopulateListsList();
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
                PopulareUrlsList();
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

        private void PopulateConfigsList()
        {
            _configurationsName = new ObservableCollection<string>();
            foreach (var item in _connections) _configurationsName.Add(item.Connection.UriString);
        }

        private void PopulateListsList()
        {
            _listsName = new ObservableCollection<string>();
            foreach (var item in _lists) _listsName.Add(item.ListName);
        }

        private void PopulareUrlsList()
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
        {
            try
            {
                var listReferenceProvider = GetListReferenceProvider();
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
            catch (Exception exception)
            {
            }

        }

        private BaseListReferenceProvider GetListReferenceProvider()
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