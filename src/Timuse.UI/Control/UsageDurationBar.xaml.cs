using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Timuse.UI.Control;

public sealed partial class UsageDurationBar : UserControl
{
    public UsageDurationBar()
    {
        this.InitializeComponent();
    }

    public string AppName { get; set; }

    /// <summary>
    /// from 0 to 1.
    /// </summary>
    public double Ratio { get; set; }

    public TimeSpan Duration { get; set; }

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
        return baseLength * this.Ratio;
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        this.iconBorder.BorderBrush = Application.Current.Resources["AccentBrush"] as Brush;
        this.appNameText.Foreground = Application.Current.Resources["AccentBrush"] as Brush;
        this.ratioBar.Fill = Application.Current.Resources["AccentBrush"] as Brush;
        this.durationText.Foreground = Application.Current.Resources["AccentBrush"] as Brush;
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        this.iconBorder.BorderBrush = null;
        this.appNameText.Foreground = Application.Current.Resources["ControlAltFillColorTertiaryBrush"] as Brush;
        this.ratioBar.Fill = Application.Current.Resources["ControlAltFillColorTertiaryBrush"] as Brush;
        this.durationText.Foreground = Application.Current.Resources["ControlAltFillColorTertiaryBrush"] as Brush;
    }
}
