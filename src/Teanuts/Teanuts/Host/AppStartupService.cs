using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Teanuts.Host;

internal class AppStartupService<TApplication> : IHostedService where TApplication : Application
{
    public AppStartupService(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    {
        this.serviceProvider = serviceProvider;
        applicationLifetime.ApplicationStopping.Register(() => application?.Exit());
    }

    private readonly IServiceProvider serviceProvider;
    
    private TApplication? application;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Application.Start(param =>
        {
            application = serviceProvider.GetRequiredService<TApplication>();
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
