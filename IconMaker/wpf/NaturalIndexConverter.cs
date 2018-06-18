using System;
using System.Globalization;
using System.Windows.Data;

namespace IconMaker.wpf
{
    public class NaturalIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && targetType == typeof(string))
                return (intValue + 1).ToString();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
