using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    //just for demoing
    public class OriginalValueStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            var str = value.ToString();
            return $"Original value: {str}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
