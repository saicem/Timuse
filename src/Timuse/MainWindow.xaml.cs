using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timuse.Interop;

namespace Timuse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += MainWindow_MouseMove;
            this.Activated += MainWindow_Activated;
            this.Deactivated += MainWindow_Deactivated;
        }

        private readonly Brush AlmostTransparent = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
        private readonly Brush InActivedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xE4, 0xEC, 0xF1));

        private void MainWindow_Deactivated(object? sender, EventArgs e)
        {
            this.Background = InActivedBrush;
        }

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            this.Background = AlmostTransparent;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private WindowAccentCompositor? Compositor { get; set; }

        protected override void OnSourceInitialized(EventArgs e)
        {
            Compositor = new WindowAccentCompositor(this);
            Compositor.Color = Color.FromArgb(0xE0, 0xE4, 0xEC, 0xF1);
            Compositor.IsEnabled = true;
            Compositor.SetRoundCorner();
        }

        [LibraryImport("Dwmapi.dll")]
        internal static unsafe partial int DwmSetWindowAttribute(nint hwnd, int dwAttribute, void* pvAttribute, int cbAttribute);
    }
}
