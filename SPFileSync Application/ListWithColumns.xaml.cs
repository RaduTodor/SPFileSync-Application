using Models;
using System.Collections.Generic;
using System.Windows;

namespace SPFileSync_Application
{
    public partial class ListWithColumns : Window
    {       
        private List<ListWithColumnsName> addList;
        public ListWithColumns(List<ListWithColumnsName> list)
        {
            InitializeComponent();           
            addList = list;            
        }

        private void ConfirmList(object sender, RoutedEventArgs e)
        {
            ListWithColumnsName list = new ListWithColumnsName() { ListName = listTextBox.Text, UrlColumnName = urlColumnTextBox.Text, UserColumnName = userColumnTextBox.Text };
            addList.Add(list);
            this.Close();
        }

        private void CancelList(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
