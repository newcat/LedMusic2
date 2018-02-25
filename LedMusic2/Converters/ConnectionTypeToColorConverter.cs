using LedMusic2.NodeConnection;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LedMusic2.Converters
{
    public class ConnectionTypeToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ConnectionType)value)
            {
                case ConnectionType.BOOL:
                    return new SolidColorBrush(Colors.DarkCyan);
                case ConnectionType.COLOR:
                case ConnectionType.COLOR_ARRAY:
                    return new SolidColorBrush(Colors.Gold);
                case ConnectionType.NUMBER:
                    return new SolidColorBrush(Colors.Gray);
                default:
                    return new SolidColorBrush(Colors.DarkRed);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
