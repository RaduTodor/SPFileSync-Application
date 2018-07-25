namespace SPFileSync_Application
{
    using System.Windows;
    using Models;

    public partial class EditItemListPanel
    {
        private GeneralUI _generalUI = new GeneralUI();
        private readonly ListWithColumnsName _itemToDisplay;
        private readonly Window _window;

        public EditItemListPanel(ListWithColumnsName item, Window window)
        {
            InitializeComponent();
            _itemToDisplay = item;
            listTextBox.Text = item.ListName;
            urlColumnTextBox.Text = item.UrlColumnName;
            userColumnTextBox.Text = item.UserColumnName;
            _window = window;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            _window.Show();
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            _itemToDisplay.ListName = listTextBox.Text;
            _itemToDisplay.UrlColumnName = urlColumnTextBox.Text;
            _itemToDisplay.UserColumnName = userColumnTextBox.Text;
            _window.Show();
            Close();
        }
    }
}