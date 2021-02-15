using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    public class RelativeWidthConverter : MarkupExtension, IValueConverter
    {

        public string Parts { get; set; }
        private CultureInfo ci = new CultureInfo("en-US");
        public RelativeWidthConverter()
        {
            Parts = "1";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double)value;
            return v / (double.Parse(Parts,ci));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double)value;
            return v * (double.Parse(Parts,ci));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }



}