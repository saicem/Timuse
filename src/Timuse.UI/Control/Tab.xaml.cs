using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Timuse.UI.Control;
public sealed partial class Tab : UserControl
{
    public Tab()
    {
        this.InitializeComponent();
    }

    public string Text { set; get; }

    public ImageSource ActiveIcon { set; get; }

    public ImageSource InactiveIcon { set; get; }

    public bool Active
    {
        get { return (bool)GetValue(ActiveProperty); }
        set { SetValue(ActiveProperty, value); }
    }

    public static readonly DependencyProperty ActiveProperty =
        DependencyProperty.Register("Active", typeof(bool), typeof(bool), new PropertyMetadata(null));

    public ImageSource GetCurrentIcon(bool active) => active ? ActiveIcon : InactiveIcon;

    public Brush GetContanerBackground(bool active)
        => active ? Application.Current.Resources["AccentGradient"] as Brush : new SolidColorBrush(Colors.Transparent);

    public Brush GetTextForeground(bool active)
        => active ? Application.Current.Resources["TextFillColorInverseBrush"] as Brush : Application.Current.Resources["TextFillColorSecondaryBrush"] as Brush;
}
