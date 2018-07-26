namespace Common.Helpers
{
    using System.Collections.ObjectModel;

    //TODO [CR BT] Add documentation
    public class ObservableHashSet<T> : ObservableCollection<T>
    {
        public void AddItem(T item)
        {
            if (!Contains(item)) Add(item);
        }
    }
}