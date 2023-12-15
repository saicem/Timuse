using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Timuse.UI.Controls;
using Timuse.UI.Pages;

namespace Timuse.UI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SubClassing();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(titleBar);

        lastActiveTab = generalTab;
        frame.Navigate(typeof(GeneralPage));
    }

    private Tab lastActiveTab;

    private void NavigatorTabTapped(object sender, TappedRoutedEventArgs e)
    {
        var activeTab = sender as Tab;
        if (!activeTab.Active)
        {
            Navigate(activeTab);
            if (lastActiveTab != null)
            {
                lastActiveTab.Active = false;
            }

            activeTab.Active = true;
            lastActiveTab = activeTab;
        }
    }

    private void Navigate(Tab tab)
    {
        if (tab == generalTab)
        {
            frame.Navigate(typeof(GeneralPage));
        }
        //else if (tab == this.statisticTab)
        //{
        //    frame.Navigate(typeof(StatisticPage));
        //}
        else if (tab == detailTab)
        {
            frame.Navigate(typeof(DetailPage));
        }
    }
}
