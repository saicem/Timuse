using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace Timuse.UI.Controls;

public sealed partial class UsageDurationBar : UserControl
{
    public UsageDurationBar()
    {
        InitializeComponent();
    }

    public string AppName { get; set; }

    /// <summary>
    /// from 0 to 1.
    /// </summary>
    public double Ratio { get; set; }

    public TimeSpan Duration { get; set; }

    public ImageSource Icon { get; set; }

    public static string DurationToText(TimeSpan duration)
    {
        var seconds = (int)duration.TotalSeconds;
        if (seconds < 60)
        {
            return $"{seconds}s";
        }

        var minutes = seconds / 60;
        if (minutes < 60)
        {
            return $"{minutes}min";
        }

        var hours = minutes / 60;
        minutes %= 60;
        return minutes switch
        {
            0 => $"{hours}h",
            _ => $"{hours}h {minutes}min",
        };
    }

    public double ComputeBarLength(double baseLength)
    {
        return baseLength * Ratio;
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        iconBorder.BorderBrush = Application.Current.Resources["AccentBrush"] as Brush;
        appNameText.Foreground = Application.Current.Resources["AccentBrush"] as Brush;
        ratioBar.Fill = Application.Current.Resources["AccentBrush"] as Brush;
        durationText.Foreground = Application.Current.Resources["AccentBrush"] as Brush;
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        iconBorder.BorderBrush = null;
        appNameText.Foreground = Application.Current.Resources["TextFillColorSecondaryBrush"] as Brush;
        ratioBar.Fill = Application.Current.Resources["ControlStrongStrokeColorDefaultBrush"] as Brush;
        durationText.Foreground = Application.Current.Resources["TextFillColorSecondaryBrush"] as Brush;
    }
}
