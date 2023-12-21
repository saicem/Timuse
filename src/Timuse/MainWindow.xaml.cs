using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Timuse;

public partial class MainWindow : Window
{
    public MainWindow(IHostApplicationLifetime applicationLifetime)
    {
        InitializeComponent();
        this.applicationLifetime = applicationLifetime;
    }

    private readonly IHostApplicationLifetime applicationLifetime;

    override protected void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        applicationLifetime.StopApplication();
    }
}
