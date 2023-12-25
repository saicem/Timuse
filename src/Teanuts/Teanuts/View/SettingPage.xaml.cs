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
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Teanuts.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }

        public async Task RenderElemrntToImage(UIElement element, FileInfo outputPng)
        {
            if (outputPng.Exists) outputPng.Create(); 
            
            using var file = File.OpenWrite(outputPng.FullName);

            var buffer = new InMemoryRandomAccessStream();
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(element);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, buffer);
            var pixcels = await renderTargetBitmap.GetPixelsAsync();
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)renderTargetBitmap.PixelWidth, (uint)renderTargetBitmap.PixelHeight, 1/96, 1/96, pixcels.ToArray());
            await encoder.FlushAsync();

            buffer.AsStream().CopyTo(file);
        }
    }
}
