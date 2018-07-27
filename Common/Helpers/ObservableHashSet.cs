namespace Common.Helpers
{
    using System.Collections.ObjectModel;

    /// <summary>
    ///     A generic HashSet collection for keeping up to date the UI parts. 
    /// </summary>
    public class ObservableHashSet<T> : ObservableCollection<T>
    {
        public void AddItem(T item)
        {
            if (!Contains(item)) Add(item);
        }


    }
}