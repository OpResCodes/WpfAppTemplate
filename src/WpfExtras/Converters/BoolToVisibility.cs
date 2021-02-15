using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    /// <summary>
    /// This is a smart B2V Converter, No need to be placed into a resource dictionary
    /// Usage in XAML:
    /// Visibility="{Binding xy, Converter={attalo:Bool2Visibility FalseVisibility=Hidden,TrueVisibility=Visibile}}"
    /// </summary>
    public class Bool2Visibility : MarkupExtension, IValueConverter
    {

        public Bool2Visibility()
        {
            FalseVisibility = Visibility.Collapsed;
            TrueVisibility = Visibility.Visible;
        }

        public Visibility FalseVisibility { get; set; }

        public Visibility TrueVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bVal;
            bool result = bool.TryParse(value.ToString(), out bVal);
            if (!result)
                return value;

            return (bVal) ? TrueVisibility : FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}