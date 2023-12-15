using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.IO;
using Timuse.UI.DataContexts;
using Timuse.UI.ViewModels;

namespace Timuse.UI;

public partial class App : Application
{
    private const string AppInstanceKey = nameof(AppInstanceKey);

    public static AppUsageModel AppUsageModel { get; private set; } = new AppUsageModel();

    public App()
    {
        InitializeComponent();

        // InitializeService();
    }

    private static void InitializeService()
    {
        var toPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "TimuseService.exe");
        var fromPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledPath, "Assets\\Bin\\TimuseService.exe");

        Process.Start(fromPath);
        if (File.Exists(toPath))
        {
            return;
        }

        File.CreateSymbolicLink(toPath, fromPath);
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

        window = new MainWindow();
        window.Activate();
    }

    private Window window;
}
