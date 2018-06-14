using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IconMaker.wpf
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string arg)
            {
                string[] parts = arg.Split(";".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    if (Enum.TryParse(parts[0], out Visibility trueValue))
                        TrueValue = trueValue;

                    if (Enum.TryParse(parts[1], out Visibility falseValue))
                        FalseValue = falseValue;
                }
            }
            if (value is bool boolValue && targetType == typeof(Visibility))
                return boolValue ? TrueValue : FalseValue;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
