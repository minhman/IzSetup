using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IzSetup.Converters;

/// <summary>
/// Converts integer screen index to visibility
/// </summary>
public class ScreenVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int screenIndex && parameter is string paramStr && int.TryParse(paramStr, out int targetScreen))
        {
            return screenIndex == targetScreen ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
