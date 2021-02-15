using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{

    public class EmptyToVisibility : MarkupExtension, IValueConverter
    {

        public EmptyToVisibility()
        {
            EmptyVisibility = Visibility.Collapsed;
            NonEmptyVisibility = Visibility.Visible;
        }

        public Visibility EmptyVisibility { get; set; }
        public Visibility NonEmptyVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return EmptyVisibility;
            else
            {
                var x = value as IEnumerable;
                var hasFirstElement = x.GetEnumerator().MoveNext();
                if (hasFirstElement)
                    return NonEmptyVisibility;
                else
                    return EmptyVisibility;
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