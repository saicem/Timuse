using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Timuse.UI.Control;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Timuse.UI.Page;

public sealed partial class GeneralPage : Microsoft.UI.Xaml.Controls.Page
{
    public GeneralPage()
    {
        this.InitializeComponent();

        // todo define reload when the page is reload by frame
        this.FillAppUsageBox();
    }

    private void FillAppUsageBox()
    {
        var todayUsage = App.DataLoader.TodayUsage.ToArray();
        if (todayUsage.Length == 0)
        {
            return;
        }

        var baseDuration = todayUsage.First().Duration.TotalMilliseconds;
        foreach (var item in todayUsage)
        {
            // todo custom filter
            var ratio = item.Duration.TotalMilliseconds / baseDuration;
            if (ratio < 0.02)
            {
                return;
            }

            this.appUsageTodayBox.Children.Add(new UsageDurationBar
            {
                AppName = item.AppName,
                Duration = item.Duration,
                Ratio = ratio,
            });
        }
    }
}
