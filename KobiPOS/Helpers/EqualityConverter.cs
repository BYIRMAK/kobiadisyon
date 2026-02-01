using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace KobiPOS.Helpers
{
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            // Check if any value is unset (binding failed)
            if (values.Any(v => v == null || v == DependencyProperty.UnsetValue))
                return false;

            return Equals(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("EqualityConverter does not support ConvertBack.");
        }
    }
}
