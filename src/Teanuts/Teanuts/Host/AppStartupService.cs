using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading;
using System.Threading.Tasks;
using WinRT;

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
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        }

        ApplicationHost.XamlCheckProcessRequirements();

        ComWrappersSupport.InitializeComWrappers();

        Application.Start(param =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            application = serviceProvider.GetRequiredService<TApplication>();
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
