using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Microsoft.UI.Xaml.Media;
using Timuse.UI.Extensions;

namespace Timuse.UI.Pages;

public record AppUsageBarModel(string AppName, double Ratio, TimeSpan Duration, ImageSource Icon);

public sealed partial class GeneralPage : Microsoft.UI.Xaml.Controls.Page
{
    public ObservableCollection<AppUsageBarModel> AppUsageBarList = new();

    public GeneralPage()
    {
        InitializeComponent();

        // todo define reload when the page is reload by frame
        FillAppUsageBox();
    }

    private void FillAppUsageBox()
    {
        var usage = App.AppUsageModel.GetExactDayUsage(DateTime.Now);

        //AppUsageBarList.Add()

        //App.AppUsageModel.GetAppDetail()

        //if (todayUsage.Length == 0)
        //{
        //    return;
        //}

        var emptyBitmap = new Bitmap(48, 48);

        var appBarUsageModels = from a in usage
                                from b in a
                                let u = App.AppUsageModel.GetAppDetail(b.Key)
                                select new AppUsageBarModel(u.Name, 1, b.Value, (u.Icon?.ToBitmap() ?? emptyBitmap).ToBitmapImage())
                                into x
                                orderby x.Duration descending
                                select x;

        foreach (var x in appBarUsageModels)
        {
            AppUsageBarList.Add(x);
        }

        //var baseDuration = todayUsage.First().Duration.TotalMilliseconds;
        //foreach (var item in usage)
        //{

        //    // todo custom filter
        //    var ratio = item.Duration.TotalMilliseconds / baseDuration;
        //    if (ratio < 0.02)
        //    {
        //        return;
        //    }

        //    this.appUsageTodayBox.Children.Add(new UsageDurationBar
        //    {
        //        AppName = item.AppName,
        //        Duration = item.Duration,
        //        Ratio = ratio,
        //    });
        //}
    }
}
