using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenJournalist.Converter
{
    public class BoolVisibilityCollapseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;

            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
