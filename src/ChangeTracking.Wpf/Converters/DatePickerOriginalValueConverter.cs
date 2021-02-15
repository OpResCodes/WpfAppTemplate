using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    public class DatePickerOriginalValueConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime?)value;
            if(dateTime.HasValue)
            {
                return dateTime.Value.ToString("dd.MM.yyyy");
            }
            return value;
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
