using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    /// <summary>
    /// This is a bool to opposite converter. No need to place this into a resource dictionary
    /// Usage: IsEnabled="{Binding xy, Converter={attalo:Bool2Opposite}}"
    /// </summary>
    public class Bool2Opposite : MarkupExtension, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bVal;
            bool result = bool.TryParse(value.ToString(), out bVal);
            if (!result)
                return value;

            return !bVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bVal;
            bool result = bool.TryParse(value.ToString(), out bVal);
            if (!result)
                return value;

            return !bVal;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}