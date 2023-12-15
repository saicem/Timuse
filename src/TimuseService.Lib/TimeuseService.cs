using System.Runtime.InteropServices;
using System.Text;

namespace TimuseService;

public static class TimeuseService
{
    private static string recordingAppName = "";
    private static string recordingAppPath = "";
    private static DateTime focusStartAt;
    private static uint indexEndDays;

    private static readonly Dictionary<string, ushort> idDictionary = new();
    private static BinaryWriter? recordWriter;
    private static ushort currentMaxId;

    private static readonly string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Timuse");
    private static readonly string pathProcessMap = Path.Combine(basePath, "map.bin");
    private static readonly string pathRecord = Path.Combine(basePath, "record.bin");
    private static readonly string pathIndex = Path.Combine(basePath, "index.bin");
    private static TimeSpan DayTimeSpan = new(TimeSpan.TicksPerDay);

    [UnmanagedCallersOnly(EntryPoint = nameof(InitService))]
    public static void InitService()
    {
        Console.WriteLine("Timuse service starting...");
        Console.WriteLine("Initializing data folder...");
        InitFolder();
        Console.WriteLine("Initializing process map...");
        InitProcessMap();
        Console.WriteLine("Initializing record...");
        InitRecord();
        Console.WriteLine("Initializing index...");
        InitIndex();
        Console.WriteLine("Timuse service started.");
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(OnSwitch))]
    public static void OnSwitch(nint lpName, nint lpPath)
    {
        var now = DateTime.UtcNow;
        var name = Marshal.PtrToStringUni(lpName);
        var path = Marshal.PtrToStringUni(lpPath);

        if (name == null || path == null) return;

        HandleStatistics(now);

        focusStartAt = now;
        recordingAppName = name;
        recordingAppPath = path;
    }

    private static void InitFolder()
    {
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
    }

    private static void InitProcessMap()
    {
        using var stream = new FileStream(pathProcessMap, FileMode.OpenOrCreate, FileAccess.Read);
        using var reader = new BinaryReader(stream, Encoding.Unicode);

        ushort id = 0;
        // read exsisting application info
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            id = reader.ReadUInt16();
            var path = reader.ReadString();
            var name = reader.ReadString();
            idDictionary.Add(path, id);

            Console.WriteLine($"application {id}: {name}, path={path}");
        }
        reader.Close();
        currentMaxId = id;
    }

    private static void InitRecord()
    {
        var stream = new FileStream(pathRecord, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        stream.Seek(stream.Length, SeekOrigin.Begin);
        recordWriter = new BinaryWriter(stream, Encoding.Unicode);
    }

    private static void InitIndex()
    {
        using var stream = new FileStream(pathIndex, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        if (stream.Length == 0)
        {
            var now = DateTime.UtcNow;
            indexEndDays = now.TotalDays();
            new BinaryWriter(stream, Encoding.Unicode).Write(indexEndDays);
            Console.WriteLine($"Index file initialize with start date: {now:yyyy-MM-dd}");
        }
        else
        {
            var startDateDays = new BinaryReader(stream, Encoding.Unicode).ReadUInt32();
            var indexCount = (uint)(stream.Length / sizeof(uint));
            indexEndDays = startDateDays + indexCount - 1u;
            Console.WriteLine($"Index start date: {new DateTime(TimeSpan.TicksPerDay * startDateDays):yyyy-MM-dd}");
        }
        Console.WriteLine($"Index end date: {new DateTime(TimeSpan.TicksPerDay * indexEndDays):yyyy-MM-dd}");
    }

    private static void HandleStatistics(DateTime focusEndAt)
    {
        if (string.IsNullOrEmpty(recordingAppName)) return;
        var id = GetCurrentApplicationId();
        var focusStartDays = focusStartAt.TotalDays();
        var focusEndDays = focusEndAt.TotalDays();
        var focusStartTimeOfDay = focusStartAt.TimeOfDay;
        var focusEndTimeOfDay = focusEndAt.TimeOfDay;

        if (focusStartDays == focusEndDays)
        {
            WriteRecord(focusStartDays, id, focusStartTimeOfDay, focusEndTimeOfDay - focusStartTimeOfDay);
        }
        else
        {
            WriteRecord(focusStartDays, id, focusStartTimeOfDay, DayTimeSpan - focusStartTimeOfDay);
            for (uint day = focusStartDays + 1; day < focusEndDays; day++)
            {
                WriteRecord(day, id, TimeSpan.Zero, DayTimeSpan);
            }
            WriteRecord(focusEndDays, id, TimeSpan.Zero, focusEndTimeOfDay);
        }
    }

    private static ushort GetCurrentApplicationId()
    {
        var success = idDictionary.TryGetValue(recordingAppPath, out ushort id);
        if (!success)
        {
            currentMaxId++;
            id = currentMaxId;
            idDictionary.Add(recordingAppPath, id);
            SaveApplicationInfo(recordingAppPath, recordingAppName, id);
        }
        return id;
    }

    private static void WriteRecord(uint day, ushort id, TimeSpan startTimeOfDay, TimeSpan duration)
    {
        if (recordWriter is null) throw new NullReferenceException("Record writer null");
        TrackIndex(day);
        recordWriter.Write(new ApplicationRecord
        {
            ApplicationId = id,
            StartTimeOfDay = startTimeOfDay,
            Duration = duration,
        }.Data);
        recordWriter.Flush();
        Console.WriteLine(@$"[{startTimeOfDay:hh\:mm\:ss}] <{duration}> {id}: {recordingAppName}");
    }

    private static void SaveApplicationInfo(string path, string name, ushort id)
    {
        using var stream = File.OpenWrite(pathProcessMap);
        stream.Seek(stream.Length, SeekOrigin.Begin);
        using var writer = new BinaryWriter(stream, Encoding.Unicode);
        writer.Write(id);
        writer.Write(path);
        writer.Write(name);
        writer.Flush();

        Console.WriteLine($"new application: {name}({id}), path={path}");
    }

    private static void TrackIndex(uint todayTotalDays)
    {
        if (todayTotalDays == indexEndDays) return;
        if (todayTotalDays < indexEndDays) throw new Exception("Index file error!");
        using var stream = new FileStream(pathRecord, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        stream.Seek(stream.Length, SeekOrigin.Current);
        using var writer = new BinaryWriter(stream, Encoding.Unicode);
        var indexPosition = (uint)recordWriter!.BaseStream.Length;
        for (uint i = indexEndDays; i < todayTotalDays; i++)
        {
            writer.Write(indexPosition);
        }
        indexEndDays = todayTotalDays;
    }
}

static class DateTimeExtension
{
    public static uint TotalDays(this DateTime dateTime)
    {
        return (uint)(dateTime.Ticks / TimeSpan.TicksPerDay);
    }
}
