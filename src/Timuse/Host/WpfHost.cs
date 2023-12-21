using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Timuse.Host;

internal class WpfHost
{
    public static IHost Build<TApplication, TMainWindow>(string[] args, Action<IHostBuilder> configureHostBuilder)
        where TApplication : System.Windows.Application
        where TMainWindow : System.Windows.Window
    {
        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<TApplication>();
            services.AddSingleton<TMainWindow>();
            services.AddHostedService<AppStartupService<TApplication, TMainWindow>>();
        });

        configureHostBuilder?.Invoke(hostBuilder);

        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        }

        var host = hostBuilder.Build();
        return host;
    }
}
