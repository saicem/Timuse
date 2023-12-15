using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Timuse.UI.DataContexts;
using Timuse.UI.Models;

namespace Timuse.UI.ViewModels;

public class AppUsageModel : ObservableObject
{
    private readonly RecordLoader recordLoader;
    private readonly MapLoader mapLoader;
    private readonly IndexLoader indexLoader;
    private readonly HashSet<uint> handledDays = new();

    /// <summary>
    /// data struct: day -> hour -> app id -> usage duration.
    /// hour: 0-23 hour, 24 means one day aggregate usage time.
    /// app id: 0 means all app aggregate usage time.
    /// </summary>
    private readonly Dictionary<uint, Dictionary<ushort, TimeSpan>[]> appUsageDict = new();

    private TimeSpan baseUtcOffset = TimeZoneInfo.Local.BaseUtcOffset;

    public static uint TodayDays { get; set; } = (uint)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);

    public AppUsageModel()
    {
        var localAppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Timuse");
        recordLoader = new RecordLoader(Path.Combine(localAppPath, "record.bin"));
        mapLoader = new MapLoader(Path.Combine(localAppPath, "map.bin"));
        indexLoader = new IndexLoader(Path.Combine(localAppPath, "index.bin"));
        LoadData();
    }

    public void LoadData()
    {
        mapLoader.Load();
        indexLoader.Load();
        Console.WriteLine("Data reloaded.");
    }

    public Dictionary<ushort, TimeSpan>[] GetExactDayUsage(DateTime date)
    {
        var utcDays = (uint)(date.ToUniversalTime().Ticks / TimeSpan.TicksPerDay);

        GetUtcDayRecords(utcDays - 1);
        GetUtcDayRecords(utcDays);
        GetUtcDayRecords(utcDays + 1);

        var days = date.Ticks / TimeSpan.TicksPerDay;

        return appUsageDict[(uint)days];
    }

    public AppDetail GetAppDetail(ushort appId)
    {
        return mapLoader.GetAppDetail(appId);
    }

    public void GetUtcDayRecords(uint days)
    {
        // todo add lock
        if (handledDays.Contains(days))
        {
            return;
        }

        handledDays.Add(days);
        var indexFrom = indexLoader.FindRecordIndex(days);
        var indexEnd = indexLoader.FindRecordIndex(days + 1);
        var records = recordLoader.GetRecords(indexFrom, indexEnd);
        if (records.Count == 0)
        {
            return;
        }

        if (baseUtcOffset.Ticks == 0)
        {
            HandleOneDay(days, records);
        }
        else if (baseUtcOffset.Ticks > 0)
        {
            var nextDayIndex = records.FindIndex(x => x.StartAtTicks + baseUtcOffset.Ticks >= TimeSpan.TicksPerDay);
            if (nextDayIndex == -1)
            {
                nextDayIndex = records.Count;
            }

            HandleOneDay(days, records.GetRange(0, nextDayIndex));
            HandleOneDay(days + 1, records.GetRange(nextDayIndex, records.Count - nextDayIndex));
        }
        else
        {
            var lastDayIndex = records.FindIndex(x => x.StartAtTicks + baseUtcOffset.Ticks >= 0);
            if (lastDayIndex == -1)
            {
                lastDayIndex = records.Count;
            }

            HandleOneDay(days - 1, records.GetRange(0, lastDayIndex));
            HandleOneDay(days, records.GetRange(lastDayIndex, records.Count - lastDayIndex));
        }
    }

    private void HandleOneDay(uint days, IEnumerable<AppRecord> records)
    {
        if (!appUsageDict.ContainsKey(days))
        {
            appUsageDict[days] = new Dictionary<ushort, TimeSpan>[25];
            for (int i = 0; i < 25; i++)
            {
                appUsageDict[days][i] = new Dictionary<ushort, TimeSpan>();
            }
        }

        var dic = appUsageDict[days];
        var dailyTotalDic = dic[24];

        foreach (var record in records)
        {
            var hours = ((baseUtcOffset.Ticks + TimeSpan.TicksPerDay + record.StartAtTicks) / TimeSpan.TicksPerHour) % 24;
            dic[hours][record.AppId] = record.Duration + dic[hours].GetValueOrDefault(record.AppId, TimeSpan.Zero);
            dic[hours][0] = record.Duration + dic[hours].GetValueOrDefault(record.AppId, TimeSpan.Zero);
            dailyTotalDic[record.AppId] = record.Duration + dailyTotalDic.GetValueOrDefault(record.AppId, TimeSpan.Zero);
            dailyTotalDic[0] = record.Duration + dailyTotalDic.GetValueOrDefault(0, TimeSpan.Zero);
        }
    }
}
