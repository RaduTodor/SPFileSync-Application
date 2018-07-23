using System.Collections.ObjectModel;

namespace BusinessLogicLayer
{
    //TODO [CR BT] : This class is more a Common class than a Business class. This generic class it's helping you to add an item to an observable list which basically it's a helper class. Please move it into Common -> Helpers.
    public class ObservableHashSet<T> : ObservableCollection<T>
    {
        public void AddItem(T item)
        {
            if (!Contains(item))
            {
                base.Add(item);
            }            
        }       
    }
}
