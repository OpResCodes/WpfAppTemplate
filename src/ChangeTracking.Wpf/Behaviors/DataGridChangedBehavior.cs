using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfExtras.Behaviors
{
    public static class DataGridChangedBehavior
    {

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty;

        static DataGridChangedBehavior()
        {
            IsActiveProperty = DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(DataGridChangedBehavior), new PropertyMetadata(false, OnIsActivePropertyChanged));
        }

        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if ((bool)e.NewValue)
                {
                    dataGrid.Loaded += DataGrid_Loaded;
                }
                else
                {
                    dataGrid.Loaded -= DataGrid_Loaded;
                }
            }
            //else if(d is Telerik.Windows.Controls.RadGridView radDataGrid)
            //{
            //    if((bool)e.NewValue)
            //    {
            //        radDataGrid.Loaded += DataGrid_Loaded;
            //    }
            //    else
            //    {
            //        radDataGrid.Loaded -= DataGrid_Loaded;
            //    }
            //}
        }

        private static void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // textboxes in datagrids are generated after loading the grid, so the styles should not be set in xaml..
            
            if(sender is DataGrid dataGrid)
            {
                foreach (var textColumn in dataGrid.Columns.OfType<DataGridTextColumn>())
                {
                    Binding binding = textColumn.Binding as Binding;
                    if (binding != null)
                    {
                        textColumn.EditingElementStyle = CreateEditingElementStyle(dataGrid, binding.Path.Path);
                        textColumn.ElementStyle = CreateElementStyle(dataGrid, binding.Path.Path);
                    }
                }
            }
        }

        private static Style CreateElementStyle(DataGrid dataGrid, string bindingPath)
        {
            //var baseStyle = dataGrid.FindResource(typeof(TextBlock)) as Style;
            var baseStyle = dataGrid.FindResource("TextBlockWithChanges") as Style;
            var style = new Style(typeof(TextBlock), baseStyle);
            AddSetters(style, bindingPath, dataGrid);
            return style;
        }

        private static Style CreateEditingElementStyle(DataGrid dataGrid, string bindingPath)
        {
            //var baseStyle = dataGrid.FindResource(typeof(TextBox)) as Style;
            Style baseStyle = dataGrid.FindResource("TextBoxWithChanges") as Style;
            var style = new Style(typeof(TextBox), baseStyle);
            AddSetters(style, bindingPath, dataGrid);
            return style;
        }

        private static void AddSetters(Style style, string bindingPath, DataGrid dataGrid)
        {
            style.Setters.Add(new Setter(ChangeBehavior.IsActiveProperty, false));

            style.Setters.Add(new Setter(ChangeBehavior.IsChangedProperty,
                new Binding(bindingPath + "IsChanged")));

            style.Setters.Add(new Setter(ChangeBehavior.OriginalValueProperty,
                new Binding(bindingPath + "OriginalValue")));

            style.Setters.Add(new Setter(Validation.ErrorTemplateProperty,
                dataGrid.FindResource("ErrorInsideErrorTemplate")));

        }
    }
}
