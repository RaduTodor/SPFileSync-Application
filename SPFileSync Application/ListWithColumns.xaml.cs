using Models;
using System.Collections.Generic;
using System.Windows;

//TODO [CR BT] : Move usings inside the namespace.
//TODO [CR BT] : Remove Window inheritance.
namespace SPFileSync_Application
{
    public partial class ListWithColumns : Window
    {
        //TODO [CR BT] : Private proprties should be named starting with "_".
        //TODO [CR BT] : Rename "addList" variable to a more speciffic name.
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
