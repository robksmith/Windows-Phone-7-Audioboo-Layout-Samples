using System;
using System.Windows;
using System.Windows.Data;

namespace AudioBoo.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
        }

        public enum ConversionFunction { TrueToVisible, TrueToInvisible }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var param = Enum.Parse(typeof(ConversionFunction), (string)parameter, true);

            bool invert = ConversionFunction.TrueToInvisible.Equals((ConversionFunction)param);
            bool sourceValue = true.Equals(value);

            return ((sourceValue != invert)) ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isVisible = Visibility.Visible.Equals(value);
            bool isInverted = ConversionFunction.TrueToVisible.Equals(parameter);

            return ((isInverted && !isVisible) || (!isInverted && isVisible));
        }
    }
}
