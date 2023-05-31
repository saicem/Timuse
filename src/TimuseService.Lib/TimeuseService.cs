namespace TimuseService;

using System.Runtime.InteropServices;
using System.Text;

public static class TimeuseService
{
    [UnmanagedCallersOnly(EntryPoint = nameof(OnSwitch))]
    public static void OnSwitch(nint lpName, nint lpPath)
    {
        var time = DateTime.UtcNow;
        var name = Marshal.PtrToStringUni(lpName);
        var path = Marshal.PtrToStringUni(lpPath);

        if (name == null || path == null) return;

        HandleStatistics();

        recordingApplicationName = name;
        recordingApplicationFocusTime = time;
        recordingApplicationPath = path;
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(InitService))]
    public static void InitService()
    {
        Console.WriteLine("Timuse service starting...");

        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        using var stream = new FileStream(pathProcessMap, FileMode.OpenOrCreate, FileAccess.Read);
        using var reader = new BinaryReader(stream, Encoding.UTF8);

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

        var recordStream = new FileStream(pathRecord, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        recordStream.Seek(recordStream.Length, SeekOrigin.Begin);
        recordWriter = new BinaryWriter(recordStream, Encoding.UTF8);

        Console.WriteLine("Timuse service started.");
    }

    private static string recordingApplicationName = "";
    private static string recordingApplicationPath = "";
    private static DateTime recordingApplicationFocusTime;

    private static readonly DateTime timeStampOrigin = new (2000, 1, 1);
    private static readonly Dictionary<string, ushort> idDictionary = new();
    private static BinaryWriter? recordWriter;
    private static ushort currentMaxId;

    private static readonly string basePath = Path.Join(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), "Local/Timuse");
    private static readonly string pathProcessMap = Path.Join(basePath, "map.bin");
    private static readonly string pathRecord = Path.Join(basePath, "record.bin");

    private static void HandleStatistics()
    {
        if (recordWriter is null) return;
        if (string.IsNullOrEmpty(recordingApplicationName)) return;

        var timestamp = (uint)(recordingApplicationFocusTime - timeStampOrigin).TotalSeconds;
        var duration = (uint)(DateTime.UtcNow - recordingApplicationFocusTime).TotalMilliseconds;

        var success = idDictionary.TryGetValue(recordingApplicationPath, out ushort id);
        if (!success)
        {
            currentMaxId++;
            id = currentMaxId;
            idDictionary.Add(recordingApplicationPath, id);
            SaveApplicationInfo(recordingApplicationPath, recordingApplicationName, id);
        }

        recordWriter.Write(id);
        recordWriter.Write(timestamp);
        recordWriter.Write(duration);
        recordWriter.Flush();
        
        Console.WriteLine($"{recordingApplicationName}({id}):{timestamp},{duration}");
    }

    private static void SaveApplicationInfo(string path, string name, ushort id)
    {
        using var stream = File.OpenWrite(pathProcessMap);
        stream.Seek(stream.Length, SeekOrigin.Begin);
        using var writer = new BinaryWriter(stream, Encoding.UTF8);
        writer.Write(id);
        writer.Write(path);
        writer.Write(name);
        writer.Flush();

        Console.WriteLine($"new application: {name}({id}), path={path}");
    }
}