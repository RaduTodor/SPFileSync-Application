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

namespace SPFileSync_Application
{
    /// <summary>
    /// Interaction logic for ConfigurationListsEdit.xaml
    /// </summary>
    //TODO [CR BT] : Please remove unused usings. Move them inside the namespace.
    public partial class ConfigurationListsEdit : Window
    {
        //TODO [CR BT] : Private proprties should be named starting with "_".
        //TODO [CR BT] : itemLists, items, itemsToRemove, countRemovedItems are not intuitive names, please rename them.
        List<ListWithColumnsName> itemLists;
        private ObservableCollection<ListWithColumnsName> items;
        private List<ListWithColumnsName> itemsToRemove = new List<ListWithColumnsName>();
        //TODO [CR BT] : This seems and should to be a constant. 
        private int countRemovedItems = 0;       
        public ConfigurationListsEdit(List<ListWithColumnsName> list)
        {
            InitializeComponent();
            itemLists = list;
            items = new ObservableCollection<ListWithColumnsName>(itemLists);
            items.CollectionChanged += ItemsCollectionChanged;
            itemListBox.ItemsSource = items;
            
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            countRemovedItems++;
            if(items.Count-countRemovedItems <1)
            {
                removeButton.IsEnabled = false;
            }          
        }

        private void RemoveItemList(object sender, RoutedEventArgs e)
        {         
            ListWithColumnsName selectedConfig = (ListWithColumnsName)itemListBox.SelectedItem;
            items.Remove(selectedConfig);
            itemsToRemove.Add(selectedConfig);
        }

        private void EditItemList(object sender, RoutedEventArgs e)
        {
            EditItemListPanel window = new EditItemListPanel((ListWithColumnsName)itemListBox.SelectedItem,this);
            window.Show();
            Hide();
        }
        //TODO [CR BT] : Rename method. The method it's called "Save" but the logic is to remove an item.
        //TODO [CR BT] : When you remove items from a list user for instead of foreach. Foreach will crush.
        private void Save(object sender, RoutedEventArgs e)
        {

            foreach(var item in itemsToRemove)
            {
                itemLists.Remove(item);
            }

            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
