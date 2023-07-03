using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Timuse.UI.Control;
using Timuse.UI.Page;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

namespace Timuse.UI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(titleBar);

        lastActiveTab = generalTab;
        frame.Navigate(typeof(GeneralPage));
    }

    private Tab lastActiveTab;

    private void NavigatorTabTapped(object sender, TappedRoutedEventArgs e)
    {
        var activeTab = (sender as Tab);
        if (!activeTab.Active)
        {
            navigate(activeTab);
            if (lastActiveTab != null)
            {
                lastActiveTab.Active = false;
            }
            activeTab.Active = true;
            lastActiveTab = activeTab;
        }
    }

    private void navigate(Tab tab)
    {
        if (tab == generalTab)
        {
            frame.Navigate(typeof(GeneralPage));
        }
        else if (tab == statisticTab)
        {
            //frame.Navigate(typeof(StatisticPage));
        }
    }
}
