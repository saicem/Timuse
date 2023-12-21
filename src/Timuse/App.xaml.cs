using Timuse.Host;

namespace Timuse;

public partial class App : HostedApp
{
    public App(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        InitializeComponent();
    }
}
