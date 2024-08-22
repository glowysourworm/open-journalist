using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenJournalist.Converter
{
    public class BoolVisibilityCollapseInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;

            if (value is bool)
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;

            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
