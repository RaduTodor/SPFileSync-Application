using Models;
using System;
using System.Collections.Generic;
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

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for EditItemListPanel.xaml
    /// </summary>
    public partial class EditItemListPanel : Window
    {
        private GeneralUI generalUI = new GeneralUI();
        private ListWithColumnsName itemToDisplay;
        private Window window;
        public EditItemListPanel(ListWithColumnsName item,Window window)
        {
            InitializeComponent();
            itemToDisplay = item;
            listTextBox.Text = item.ListName;
            urlColumnTextBox.Text = item.UrlColumnName;
            userColumnTextBox.Text = item.UserColumnName;
            this.window = window;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            window.Show();
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var listvalidation = generalUI.FieldValidation(listTextBox.Text, listErrorLabel);
            var urlColumnValidation = generalUI.FieldValidation(urlColumnTextBox.Text, urlColumnError);
            var userColumnValidation = generalUI.FieldValidation(userColumnTextBox.Text, userColumnError);
            if(listvalidation && urlColumnValidation && userColumnValidation)
            {
                itemToDisplay.ListName = listTextBox.Text;
                itemToDisplay.UrlColumnName = urlColumnTextBox.Text;
                itemToDisplay.UserColumnName = userColumnTextBox.Text;                
                window.Show();
                Close();
            }         
        }
    }
}
