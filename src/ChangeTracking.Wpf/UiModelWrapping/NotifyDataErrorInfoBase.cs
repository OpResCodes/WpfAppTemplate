#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChangeTracking.Wpf
{
    public class NotifyDataErrorInfoBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {

        protected readonly Dictionary<string, List<string>> Errors;

        protected NotifyDataErrorInfoBase()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        [Display(AutoGenerateField = false)]
        public bool HasErrors => Errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && Errors.ContainsKey(propertyName)
                ? Errors[propertyName] : Enumerable.Empty<string>();
        }

        protected void ClearErrors()
        {
            foreach (var propName in Errors.Keys.ToList())
            {
                Errors.Remove(propName);
                OnErrorsChanged(propName);
            }
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="backingStore">Backing store.</param>
        /// <param name="value">Value.</param>
        /// <param name="validateValue">Validates value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="onChanged">On changed.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action? onChanged = null,
            Func<T, T, bool>? validateValue = null)
        {
            //if value didn't change
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            //if value changed but didn't validate
            if (validateValue != null && !validateValue(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
