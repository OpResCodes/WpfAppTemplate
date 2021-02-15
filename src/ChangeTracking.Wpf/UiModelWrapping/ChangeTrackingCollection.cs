using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Windows;

namespace ChangeTracking.Wpf
{
    public class ChangeTrackingCollection<T>
        : ObservableCollection<T>, IValidatableTrackingObject,
        IChangeTrackerRestarter, IDisposable where T : class, IValidatableTrackingObject
    {

        #region Fields

        private List<T> _originalCollection;
        private readonly ObservableCollection<T> _addedItems;
        private readonly ObservableCollection<T> _removedItems;
        private readonly ObservableCollection<T> _modifiedItems;

        #endregion

        #region Constructor

        public ChangeTrackingCollection() : base()
        {
            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();
            _originalCollection = new List<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);

        }

        public ChangeTrackingCollection(IEnumerable<T> items) : base(items.ToArray())
        {
            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();
            _originalCollection = this.ToList();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);

            AttachItemProperyChangedHandler(_originalCollection);
        }

        #endregion

        #region Properties

        public ReadOnlyObservableCollection<T> AddedItems { get; private set; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; private set; }
        public ReadOnlyObservableCollection<T> ModifiedItems { get; private set; }

        public bool IsChanged
        {
            get
            {
                return _addedItems.Count > 0 || _removedItems.Count > 0 || _modifiedItems.Count > 0;
            }
        }

        public bool IsValid
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (!this[i].IsValid)
                        return false;
                }
                return true;
            }
        }

        public bool IsChangeTracking { get; set; } = true;

        public event PropertyChangedEventHandler ItemChanged;

        #endregion

        #region Public Methods

        public void RestartChangeTracking()
        {
            DetermineCollectionChanges();//expensive method
            NotifyIsChangedChanged();
            NotifyIsValidChanged(true);
            NotifyItemsChanged();
            IsChangeTracking = true;
        }

        public void StopChangeTracking()
        {
            IsChangeTracking = false;
        }


        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (!collection.Any())
                return;

            int stIdx = Count;
            CheckReentrancy();
            foreach (T item in collection)
            {
                Items.Add(item);
            }
            List<T> changedItems = new List<T>(collection);
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(
                action: NotifyCollectionChangedAction.Add,
                changedItems: changedItems,
                startingIndex: stIdx);
            OnCollectionChanged(e);
        }

        public void AcceptChanges()
        {
            bool wasValid = IsValid;

            foreach (T item in this)
            {
                item.AcceptChanges();
            }
            _originalCollection = this.ToList();

            Application.Current.Dispatcher.Invoke(( ) =>
            {
                ClearSubCollections();
                NotifyIsChangedChanged();
                NotifyItemsChanged();
                NotifyIsValidChanged(wasValid);
            });
        }

        public void RejectChanges()
        {
            bool wasValid = IsValid;

            //Reset Added
            for (int i = 0; i < _addedItems.Count; i++)
            {
                Items.Remove(_addedItems[i]);
                _addedItems[i].PropertyChanged -= ItemPropertyChanged;
            }
            //Reset Removed
            for (int i = 0; i < _removedItems.Count; i++)
            {
                if (_removedItems[i].IsChanged)
                {
                    _removedItems[i].RejectChanges();
                }
                Items.Add(_removedItems[i]);
                _removedItems[i].PropertyChanged += ItemPropertyChanged;
            }
            //Reset Modified
            for (int i = 0; i < _modifiedItems.Count; i++)
            {
                _modifiedItems[i].PropertyChanged -= ItemPropertyChanged;//prevent update of _modifiedItems collection
                _modifiedItems[i].RejectChanges();
                _modifiedItems[i].PropertyChanged += ItemPropertyChanged;
            }


            Application.Current.Dispatcher.Invoke(() =>
            {
                base.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                ClearSubCollections();//keep original collection as is
                NotifyIsChangedChanged();
                NotifyItemsChanged();
                NotifyIsValidChanged(wasValid);
            });
        }

        /// <summary>
        /// Detaches all ItemPropertyChanged event handlers
        /// </summary>
        /// <remarks>(not really needed if the items die with the collection)</remarks>
        public void Dispose()
        {
            DetachItemPropertyChangedHandler(Items);
        }

        #endregion

        #region Private Methods

        protected void DetermineCollectionChanges()
        {
            ClearSubCollections();
            //additions and modifications of original collection
            bool itemIsAdded;
            List<T> originalItemsNotInCurrentCollection = new List<T>(_originalCollection);
            List<T> currentItemsWithOutAdded = new List<T>(this);
            foreach (T currentItem in this)
            {
                itemIsAdded = true;
                foreach (T originalItem in _originalCollection)
                {
                    if (currentItem == originalItem)
                    {
                        itemIsAdded = false;
                        break;
                    }
                }
                if (itemIsAdded)
                {
                    _addedItems.Add(currentItem);
                    currentItemsWithOutAdded.Remove(currentItem);
                }
                else
                {   //item exists already because we found it in original collection
                    originalItemsNotInCurrentCollection.Remove(currentItem);
                    if (currentItem.IsChanged)
                    {
                        _modifiedItems.Add(currentItem);
                    }
                }
            }
            //removals from original collection
            for (int i = 0; i < originalItemsNotInCurrentCollection.Count; i++)
            {
                _removedItems.Add(originalItemsNotInCurrentCollection[i]);
            }
        }

        /// <summary>
        /// Mostly used to attach/detach the items property changed handlers
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            //track or untrack items property changes
            if (HasItems(e.NewItems))
            {
                AttachItemProperyChangedHandler(AsItemList(e.NewItems));
            }
            if (HasItems(e.OldItems))
            {
                DetachItemPropertyChangedHandler(AsItemList(e.OldItems));
            }

            if (IsChangeTracking)
            {
                DetermineCollectionChanges();//expensive method
                NotifyIsChangedChanged();
                NotifyIsValidChanged(true);
                NotifyItemsChanged();
            }

            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected bool HasItems(IList collection)
        {
            return collection != null && collection.Count > 0;
        }

        protected IEnumerable<T> AsItemList(IList collection)
        {
            foreach (T item in collection)
            {
                yield return (T)item;
            }
        }

        protected void ClearSubCollections()
        {
            _addedItems.Clear();
            _removedItems.Clear();
            _modifiedItems.Clear();
        }

        protected void AttachItemProperyChangedHandler(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                item.PropertyChanged -= ItemPropertyChanged;
                item.PropertyChanged += ItemPropertyChanged;
            }
        }

        protected void DetachItemPropertyChangedHandler(IEnumerable<T> removed)
        {
            foreach (T removedItem in removed)
            {
                removedItem.PropertyChanged -= ItemPropertyChanged;
            }
        }

        //propagate IsChanged and IsValid
        protected void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsChangeTracking)
                return;
            
            if (e.PropertyName == "IsValid")
            {
                NotifyIsValidChanged(false);
            }
            if (e.PropertyName == "IsChanged")
            {
                T item = (T)sender;
                if (!_addedItems.Contains(item))
                {
                    if (item.IsChanged)
                    {
                        if (!_modifiedItems.Contains(item))
                        {
                            _modifiedItems.Add(item);
                        }
                    }
                    else
                    {
                        if (_modifiedItems.Contains(item))
                        {
                            _modifiedItems.Remove(item);
                        }
                    }
                }
                NotifyIsChangedChanged();
            }
            ItemChanged?.Invoke(sender, e);
        }

        //protected void BroadCast(bool oldValue, bool newValue, string propertyname)
        //{
        //    Messenger.Default.Send(new PropertyChangedMessage<bool>(oldValue, newValue, propertyname));
        //}

        protected void NotifyIsValidChanged(bool oldValue)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
            //BroadCast(oldValue, IsValid, "IsValid");
        }

        protected void NotifyIsChangedChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            //BroadCast(false, IsChanged, nameof(IsChanged));
        }

        protected void NotifyItemsChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        #endregion

    }

}
