using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Timuse.UI;

public partial class App : Application
{
    private const string AppInstanceKey = nameof(AppInstanceKey);

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            AppInstance firstInstance = AppInstance.FindOrRegisterForKey(AppInstanceKey);

            if (firstInstance.IsCurrent)
            {
            }
            else
            {
                firstInstance.RedirectActivationToAsync(activatedEventArgs).AsTask().Wait();
                Process.GetCurrentProcess().Kill();
            }
        }
        catch
        {
            Process.GetCurrentProcess().Kill();
        }
        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window m_window;
}
