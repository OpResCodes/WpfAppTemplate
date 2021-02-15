using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    /// <summary>
    /// This is a smart String To Integer Converter, No need to be placed into a resource dictionary
    /// Usage in XAML:
    /// Text="{Binding xy, Converter={attalo:StringToInt}}"
    /// </summary>
    public class StringToInt : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number;
            if (int.TryParse(value.ToString(), NumberStyles.AllowThousands, new CultureInfo("en-US"), out number))
            {
                return number;
            }
            else
            {
                return value;
            }

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
