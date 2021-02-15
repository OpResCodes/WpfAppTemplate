using Attalo.WPF.Utilities.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace WpfTemplate.ViewModel
{

    public class SelectableItem<T> : ObservableObject where T : class
    {
        private T _item = null;
        private bool _isSelected = false;

        public SelectableItem(T item, bool selection = false)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            _isSelected = selection;
        }

        /// <summary>
        /// Sets and gets the Item property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public T Item
        {
            get { return _item; }
            set { Set("Item", ref _item, value); }
        }
        
        /// <summary>
        /// Sets and gets the IsSelected property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (value != _isSelected)
                {
                    RaisePropertyChanged("IsSelected");
                    _isSelected = value;
                    Messenger.Default.Send(new ItemSelectionMessage(this, _isSelected));
                }
            }
        }

    }
}
