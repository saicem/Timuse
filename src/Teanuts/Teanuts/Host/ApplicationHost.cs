using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using WinRT;

namespace Teanuts.Host;

internal static partial class ApplicationHost
{
    [LibraryImport("Microsoft.ui.xaml.dll")]
    internal static partial void XamlCheckProcessRequirements();

    public static IHost Build<TApplication, TMainWindow>(string[] args, Action<IHostBuilder> configureHostBuilder)
        where TApplication : Application
        where TMainWindow : Window
    {
        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<TApplication>();
            services.AddSingleton<TMainWindow>();
            services.AddHostedService<AppStartupService<TApplication>>();
        });

        configureHostBuilder?.Invoke(hostBuilder);

        var host = hostBuilder.Build();
        return host;
    }
}
