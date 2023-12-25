using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Teanuts.Converter;

internal class TimeSpanToHourMinConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var timeSpan = (TimeSpan)value;
        return $"{(timeSpan.Hours != 0 ? timeSpan.Hours + "h " : string.Empty)}{timeSpan.Minutes}m";
    }

    [DoesNotReturn]
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

internal class TimeSpanToRatioTransformConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var timeSpan = (TimeSpan)value;
        var longestTimeSpan = TimeSpan.FromHours(24);
        var ratio = timeSpan.Ticks / (double)longestTimeSpan.Ticks;

        return new ScaleTransform { ScaleX = ratio, ScaleY = 1 };
    }

    [DoesNotReturn]
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
