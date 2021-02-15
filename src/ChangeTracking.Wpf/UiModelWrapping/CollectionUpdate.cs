namespace ChangeTracking.Wpf
{
    public class CollectionUpdate<T>
    {

        public CollectionUpdate(T[] added, T[] modified, T[] removed)
        {
            AddedItems = added;
            ModifiedItems = modified;
            RemovedItems = removed;
        }

        public T[] AddedItems { get; }

        public T[] ModifiedItems { get; }

        public T[] RemovedItems { get; }
    }

}
