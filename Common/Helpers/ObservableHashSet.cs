using System.Collections.ObjectModel;

namespace Common.Helpers
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
