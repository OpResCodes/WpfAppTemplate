using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

namespace WpfExtras.Behaviors
{
    /// <summary>
    /// Using this behavior on a dataGRid will ensure to display only columns with AutoGenerate Attribute
    /// </summary>
    public static class ColumnAutoGenerationBehavior
    {
        public static readonly DependencyProperty IsActiveProperty;

        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        static ColumnAutoGenerationBehavior()
        {
            IsActiveProperty = DependencyProperty.RegisterAttached(
                "IsActive", typeof(bool),
                typeof(ColumnAutoGenerationBehavior),
                new PropertyMetadata(false, OnIsActivePropertyChanged));
        }

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = d as DataGrid;
            if (dataGrid != null)
            {
                if ((bool)e.NewValue)
                {
                    dataGrid.AutoGeneratingColumn += DataGridOnAutoGeneratingColumn;
                }
                else
                {
                    dataGrid.AutoGeneratingColumn -= DataGridOnAutoGeneratingColumn;
                }
            }
        }

        private static void DataGridOnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var propDesc = e.PropertyDescriptor as PropertyDescriptor;
            if (propDesc != null)
            {
                foreach (Attribute att in propDesc.Attributes)
                {
                    var displayAttribute = att as DisplayAttribute;
                    if (displayAttribute != null)
                    {
                        var auto = displayAttribute.GetAutoGenerateField();
                        if (auto != null && !displayAttribute.AutoGenerateField)
                        {
                            e.Cancel = true;
                        }
                        var name = displayAttribute.GetName();
                        var shortName = displayAttribute.GetShortName();
                        if (!string.IsNullOrEmpty(name))
                            e.Column.Header = name;
                        if (!string.IsNullOrEmpty(shortName))
                            e.Column.Header = shortName;
                    }
                    var displayName = att as DisplayNameAttribute;
                    if (displayName != null)
                    {
                        e.Column.Header = displayName.DisplayName;
                    }
                }
            }
        }
    }


}
