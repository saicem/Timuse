using Microsoft.Extensions.Hosting;

namespace Timuse.Host;

internal class AppStartupService<TApplication, TMainWindow> : IHostedService
    where TApplication : System.Windows.Application
    where TMainWindow : System.Windows.Window
{
    public AppStartupService(TApplication application, TMainWindow mainWindow, IHostApplicationLifetime applicationLifetime)
    {
        this.application = application;
        this.mainWindow = mainWindow;

        applicationLifetime.ApplicationStopping.Register(() => application.Shutdown(Environment.ExitCode));
    }

    private readonly TApplication application;
    private readonly TMainWindow mainWindow;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        application.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
        application.Run(mainWindow);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
