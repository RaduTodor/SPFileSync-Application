
namespace SPFileSync_Application
{
    using Models;
    using System.Windows;

    public partial class EditItemListPanel
    {
        private GeneralUI _generalUI = new GeneralUI();
        private ListWithColumnsName _itemToDisplay;
        private Window _window;
        public EditItemListPanel(ListWithColumnsName item,Window window)
        {
            InitializeComponent();
            _itemToDisplay = item;
            listTextBox.Text = item.ListName;
            urlColumnTextBox.Text = item.UrlColumnName;
            userColumnTextBox.Text = item.UserColumnName;
            this._window = window;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            _window.Show();
            Close();
        }
        //TODO [CR BT] : Move validations into ConfigurationValidator class.
        private void Save(object sender, RoutedEventArgs e)
        {
            var listvalidation = _generalUI.FieldValidation(listTextBox.Text, listErrorLabel);
            var urlColumnValidation = _generalUI.FieldValidation(urlColumnTextBox.Text, urlColumnError);
            var userColumnValidation = _generalUI.FieldValidation(userColumnTextBox.Text, userColumnError);
            if(listvalidation && urlColumnValidation && userColumnValidation)
            {
                _itemToDisplay.ListName = listTextBox.Text;
                _itemToDisplay.UrlColumnName = urlColumnTextBox.Text;
                _itemToDisplay.UserColumnName = userColumnTextBox.Text;                
                _window.Show();
                Close();
            }         
        }
    }
}
