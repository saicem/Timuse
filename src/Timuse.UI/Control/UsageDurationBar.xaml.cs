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
using Windows.UI;

namespace Timuse.UI.Control;
public sealed partial class UsageDurationBar : UserControl
{
    public UsageDurationBar()
    {
        this.InitializeComponent();
    }

    public string AppName { get; set; }

    /// <summary>
    /// from 0 to 1
    /// </summary>
    public double Ratio { get; set; }

    public TimeSpan Duration { get; set; }

    public static string DurationToText(TimeSpan Duration)
    {
        var seconds = (int)Duration.TotalSeconds;
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
        if(minutes == 0)
        {
            return $"{hours}h";
        }
        return $"{hours}h {minutes}min";
    }

    public double ComputeBarLength(double baseLength, double ratio)
    {
        return baseLength * ratio;
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        iconBorder.BorderBrush = new SolidColorBrush { Color = Color.FromArgb(255, 57, 106, 252) };
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        iconBorder.BorderBrush = null;
    }
}
