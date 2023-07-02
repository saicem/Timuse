using ABI.Microsoft.UI.Xaml;
using Microsoft.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timuse.UI.Data;

public class DataLoader
{
    private string recordBinPath;
    private string mapBinPath;
    private Dictionary<ushort, (string, string)> appMap = new();
    private List<ApplicationRecord> appRecords = new();

    public IEnumerable<AppUsageDuration> TodayUsage { get => ComputeAppUsageDuration(appRecords); }

    public DataLoader()
    {
        var localAppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Timuse");
        recordBinPath = Path.Combine(localAppPath, "record.bin");
        mapBinPath = Path.Combine(localAppPath, "map.bin");
        Reload();
    }

    public void Reload()
    {
        LoadMapData();
        LoadRecordData();
        Console.WriteLine("Data reloaded.");
    }

    public void LoadRecordData()
    {
        var stream = new FileStream(recordBinPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
        var reader = new BinaryReader(stream, Encoding.UTF8);

        var records = new List<ApplicationRecord>();
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            records.Add(new ApplicationRecord { Data = reader.ReadUInt64() });
        }
        this.appRecords = records;
        stream.Close();
    }

    public void LoadMapData()
    {

        using var stream = new FileStream(mapBinPath, FileMode.OpenOrCreate, FileAccess.Read);
        using var reader = new BinaryReader(stream, Encoding.UTF8);

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            ushort id = reader.ReadUInt16();
            var path = reader.ReadString();
            var name = reader.ReadString();
            appMap.Add(id, (name, path));
        }
        reader.Close();
    }

    public IEnumerable<AppUsageDuration> ComputeAppUsageDuration(IEnumerable<ApplicationRecord> records)
    {
        return from record in records
               group record by record.ApplicationId into g
               select new
               {
                   AppInfo = appMap.GetValueOrDefault(g.Key),
                   DurationTicks = g.Sum(r => r.DurationTTicks),
               }
               into x
               orderby x.DurationTicks descending
               select new AppUsageDuration
               {
                   AppName = x.AppInfo.Item1,
                   AppPath = x.AppInfo.Item2,
                   Duration = ApplicationRecord.TTicksToTimeSpan(x.DurationTicks),
               };
    }
}

public struct AppUsageDuration
{
    public string AppName;
    public string AppPath;
    public TimeSpan Duration;
}
