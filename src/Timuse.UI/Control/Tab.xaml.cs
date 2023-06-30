using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
                //border.Background = control.Resources["BackgroundActive"] as Brush;
                text.Foreground = new SolidColorBrush { Color = Colors.White };
            }
            else
            {
                icon.Source = new SvgImageSource(new Uri(InactiveIcon));
                border.Background = null;
                text.Foreground = new SolidColorBrush { Color = Color.FromArgb(255, 125, 135, 165) };
            }
        }
        get { return active; }
    }

    public string CurrentIcon { get => active ? ActiveIcon : InactiveIcon; }
}
