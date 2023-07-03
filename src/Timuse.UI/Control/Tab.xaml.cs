using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace Timuse.UI.Control;
public sealed partial class Tab : UserControl
{
    public Tab()
    {
        this.InitializeComponent();
    }

    public string Text { set; get; }

    public string ActiveIcon { set; get; }

    public string InactiveIcon { set; get; }

    private bool active;

    public bool Active
    {
        set
        {
            active = value;
            if (active)
            {
                icon.Source = new SvgImageSource(new Uri(ActiveIcon));
                container.Background = Application.Current.Resources["SCB1"] as Brush;
                text.Foreground = Application.Current.Resources["SCB2"] as Brush;
            }
            else
            {
                icon.Source = new SvgImageSource(new Uri(InactiveIcon));
                container.Background = new SolidColorBrush(Colors.Transparent);
                text.Foreground = Application.Current.Resources["SCB4"] as Brush;
            }
        }
        get { return active; }
    }

    public string CurrentIcon { get => active ? ActiveIcon : InactiveIcon; }
}
