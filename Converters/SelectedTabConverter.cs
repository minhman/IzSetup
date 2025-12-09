using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace IzSetup.Converters;

/// <summary>
/// Converts tab name to background color based on selection
/// </summary>
public class SelectedTabConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var currentTab = value as string;
        var tabName = parameter as string;

        if (currentTab == tabName)
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"));
        else
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
