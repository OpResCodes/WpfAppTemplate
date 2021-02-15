using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    public class StringShortener : MarkupExtension, IValueConverter
    {
        public int MaxLength { get; set; }

        public StringShortener()
        {
            MaxLength = 30;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            if (str.Length > MaxLength)
            {
                str = string.Format("{0}...", str.Substring(0, MaxLength));
            }
            return str;
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
