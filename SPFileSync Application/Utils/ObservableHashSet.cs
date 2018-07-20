using System.Collections.ObjectModel;

namespace SPFileSync_Application.Utils
{
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
