using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using Teanuts.ViewModel.Control;

namespace Teanuts.View;

public sealed partial class OverviewPage : Page
{
    public OverviewPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DayItems.Add(new AppicationUsage { Name = "Visual Studio", Duration = TimeSpan.FromMinutes(30) });
        DayItems.Add(new AppicationUsage { Name = "Firefox", Duration = TimeSpan.FromMinutes(134) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft Edge", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Chrome", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft Word", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft Excel", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft PowerPoint", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft Teams", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft Outlook", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft OneNote", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Microsoft OneDrive", Duration = TimeSpan.FromMinutes(20) });
        DayItems.Add(new AppicationUsage { Name = "Visual Studio Code", Duration = TimeSpan.FromMinutes(20) });
        DailyItemsCVS.Source = DayItems;
    }

    public ObservableCollection<AppicationUsage> DayItems { get; } = [];
}
