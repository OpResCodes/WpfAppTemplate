using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    /// <summary>
    /// This is a smart Null to Visibility Converter, No need to be placed into a resource dictionary
    /// Usage in XAML:
    /// Visibility="{Binding xy, Converter={attalo:Null2Visibility}}"
    /// </summary>
    public class Null2Visibility : MarkupExtension, IValueConverter
    {

        public Null2Visibility()
        {
            NullVisibility = Visibility.Collapsed;
            NonNullVisibility = Visibility.Visible;
        }

        public Visibility NullVisibility { get; set; }
        public Visibility NonNullVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullVisibility;
            else
                return NonNullVisibility;
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