using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace IzSetup.Converters;

/// <summary>
/// Converts installation status to visibility for version info
/// </summary>
public class StatusToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value as string;
        var targetStatus = parameter as string;

        if (status == targetStatus)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
