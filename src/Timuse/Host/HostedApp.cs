namespace Timuse.Host;

public class HostedApp(IServiceProvider serviceProvider) : System.Windows.Application
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public static new HostedApp Current => (HostedApp)System.Windows.Application.Current;
}
