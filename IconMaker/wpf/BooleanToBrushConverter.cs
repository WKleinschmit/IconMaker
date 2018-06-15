using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IconMaker.wpf
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush TrueValue { get; set; }
        public Brush FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && targetType == typeof(Brush))
                return boolValue ? TrueValue : FalseValue;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
