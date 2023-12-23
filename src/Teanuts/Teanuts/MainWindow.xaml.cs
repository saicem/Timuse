using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using Teanuts.Extension;

namespace Teanuts;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow(IHostApplicationLifetime applicationLifetime)
    {
        this.InitializeComponent();
        this.LoadWebview();

        this.ExtendsContentIntoTitleBar = true;

        AppTitleBar.Loaded += this.OnAppTitleBarLoaded;

        this.Closed += (s, e) =>
        {
            applicationLifetime.StopApplication();
        };
        this.TrySetMicaBackdrop(useMicaAlt: false);
    }

    private async void LoadWebview()
    {
        AppWebView.CoreWebView2Initialized += this.OnCoreWebView2Initialized;
        await AppWebView.EnsureCoreWebView2Async();
    }

    private void OnCoreWebView2Initialized(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
    {
        AppWebView.NavigateToString(
            """
            <!DOCTYPE html>
            <html>
                <head>
                <meta charset='utf-8'>
                <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                <title>Page Title</title>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
                <style>
                    body {
                        background-color: rgba(0, 0, 0, 0);
                        color: #fff;
                    }
                </style>
                </head>
                <body>
                <h1>This is a Heading</h1>
                <p>This is a paragraph.</p>
                </body>
            </html>
            """);
    }

    private void OnAppTitleBarLoaded(object sender, RoutedEventArgs e)
    {
        if (!ExtendsContentIntoTitleBar) return;
        this.SetTitleBar(AppTitleHeader);
    }
}
