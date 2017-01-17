using LedMusic2.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LedMusic2.Converters
{
    class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LedColor)
            {
                var c = ((LedColor)value).getColorRGB();
                return new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
            } else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
