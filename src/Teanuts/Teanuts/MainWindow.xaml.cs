using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using Teanuts.Extension;
using Teanuts.View;

namespace Teanuts;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow(IHostApplicationLifetime applicationLifetime)
    {
        this.InitializeComponent();

        this.ExtendsContentIntoTitleBar = true;

        AppTitleBar.Loaded += this.OnAppTitleBarLoaded;
        AppNavigationView.Loaded += this.OnAppNavigationViewLoaded;

        this.Closed += (s, e) =>
        {
            applicationLifetime.StopApplication();
        };
        this.TrySetMicaBackdrop(useMicaAlt: true);
    }

    private static Dictionary<string, (Type PageType, string NavigationParamter)> NavigationMap = new()
    {
        { "Home", (typeof(OverviewPage), string.Empty) },
        { "TimeLine", (typeof(TimeLinePage), string.Empty) },
        { "Fragment", (typeof(FragmentPage), string.Empty) },
        { "Catalog", (typeof(CatalogPage), string.Empty) }
    };

    private void NavigateToPage(Type navPageType, string? navParamter, NavigationTransitionInfo transitionInfo)
    {
        if (navPageType == null) return;
        var prevPageType = ContentFrame.CurrentSourcePageType;
        if (navPageType == prevPageType && (string)ContentFrame.Tag == navParamter) return;

        ContentFrame.Navigate(navPageType, navParamter, transitionInfo);
        ContentFrame.Tag = navParamter;
    }

    private void OnAppNavigationViewLoaded(object sender, RoutedEventArgs e)
    {
        AppNavigationView.SelectionChanged += this.OnNavigationSelectionChanged;
        AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
    }

    private void OnNavigationSelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            NavigateToPage(typeof(SettingPage), null, args.RecommendedNavigationTransitionInfo);
        }
        else if (args.SelectedItemContainer != null)
        {
            var navTag = args.SelectedItemContainer.Tag.ToString();
            if (string.IsNullOrEmpty(navTag)) return;

            var navTypeExists = NavigationMap.TryGetValue(navTag, out var navPageInfo);
            if (!navTypeExists) return;

            NavigateToPage(navPageInfo.PageType, navPageInfo.NavigationParamter, args.RecommendedNavigationTransitionInfo);
        }
        if (args.SelectedItemContainer is not NavigationViewItem navItem) return;

        IconSource? iconSource = navItem.Icon switch
        {
            SymbolIcon symbolIcon => new SymbolIconSource { Symbol = symbolIcon.Symbol },
            AnimatedIcon animatedIcon => new AnimatedIconSource { Source = animatedIcon.Source },
            _ => null
        };

        HomeTab.IconSource = iconSource;
        HomeTab.Header = navItem.Content;
        HomeTab.Tag = navItem;
    }

    private void OnAppTitleBarLoaded(object sender, RoutedEventArgs e)
    {
        if (!ExtendsContentIntoTitleBar) return;
        this.SetTitleBar(AppTitleHeader);
    }
}
