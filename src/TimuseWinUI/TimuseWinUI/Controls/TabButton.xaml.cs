using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI;
using Windows.UI;

namespace TimuseWinUI.Controls
{
    public sealed partial class TabButton : UserControl
    {
        public TabButton()
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
                    border.Background = control.Resources["BackgroundActive"] as Brush;
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
}
