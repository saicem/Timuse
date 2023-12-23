using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace Teanuts;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App(IServiceProvider serviceProvider)
    {
        this.InitializeComponent();
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        this.m_window = serviceProvider.GetRequiredService<MainWindow>();
        m_window.Activate();
    }

    private readonly IServiceProvider serviceProvider;
    private Window? m_window;
}
