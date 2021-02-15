using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfExtras
{
    public class ComboBoxOriginalValueConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            var comboBox = parameter as ComboBox;
            if (comboBox != null && comboBox.ItemsSource != null)
            {
                var lookupItem
                  = comboBox.ItemsSource.OfType<ILookupItem>().SingleOrDefault(l => l.Id == id);
                if (lookupItem != null)
                {
                    return string.Format("Original Value: {0}",lookupItem.Label);
                }
            }
            //else if(parameter is Telerik.Windows.Controls.RadComboBox rcb)
            //{
            //    if(rcb.ItemsSource != null)
            //    {
            //        var lookupItem = rcb.ItemsSource.OfType<LookupItem>().SingleOrDefault(l => l.Id == id);
            //        if (lookupItem != null)
            //        {
            //            return string.Format("Original Value: {0}", lookupItem.Label);
            //        }
            //    }
            //}
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
