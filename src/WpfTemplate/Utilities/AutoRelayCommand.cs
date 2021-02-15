using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace WpfTemplate.Utilities
{
    public class AutoRelayCommand : RelayCommand
    {
        private ISet<string> properties;

        public AutoRelayCommand(Action execute) : base(execute)
        {
            Initialize();
        }

        public AutoRelayCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
            Initialize();
        }

        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
            {
                if (properties != null && properties.Contains(property.PropertyName))
                    this.RaiseCanExecuteChanged();
            });
        }

        public void DependsOn<T>(Expression<Func<T>> propertyExpression)
        {
            if (properties == null)
                properties = new HashSet<string>();

            properties.Add(this.GetPropertyName(propertyExpression));
        }

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", "propertyExpression");

            var property = body.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Argument is not a property",
                    "propertyExpression");

            return property.Name;
        }

        public void CleanUp()
        {
            Messenger.Default.Unregister(this);
        }

    }

    public class AutoRelayCommand<T> : RelayCommand<T>
    {

        private ISet<string> properties;

        public AutoRelayCommand(Action<T> execute) : base(execute)
        {
            Initialize();
        }

        public AutoRelayCommand(Action<T> execute, Func<T, bool> canExecute) : base(execute, canExecute)
        {
            Initialize();
        }

        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
            {
                if (properties != null && properties.Contains(property.PropertyName))
                    RaiseCanExecuteChanged();
            });
        }

        public void DependsOn<TProp>(Expression<Func<TProp>> propertyExpression)
        {
            if (properties == null)
                properties = new HashSet<string>();

            properties.Add(GetPropertyName(propertyExpression));
        }

        private string GetPropertyName<TProp>(Expression<Func<TProp>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", "propertyExpression");

            var property = body.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Argument is not a property", "propertyExpression");

            return property.Name;
        }

        public void CleanUp()
        {
            Messenger.Default.Unregister(this);
        }
    }

}
