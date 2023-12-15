using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Timuse.UI.Models;

namespace Timuse.UI.DataContexts;

internal class MapLoader
{
    private readonly string filePath;
    private Dictionary<ushort, string> appMap;

    internal MapLoader(string filePath)
    {
        this.filePath = filePath;
    }

    internal void Load()
    {
        using var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
        using var reader = new BinaryReader(stream, Encoding.UTF8);

        var arr = new List<(ushort, string)>();
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            ushort id = reader.ReadUInt16();
            var path = reader.ReadString();
            var name = reader.ReadString();
            arr.Add((id, path));
        }

        appMap = arr.ToDictionary(x => x.Item1, x => x.Item2);
        reader.Close();
    }

    internal AppDetail GetAppDetail(ushort id)
    {
        var have = appMap.TryGetValue(id, out var path);
        if (!have)
        {
            Load();
            have = appMap.TryGetValue(id, out path);
            if (!have)
            {
                throw new Exception($"App id: {id} could not be find in map.bin.");
            }
        }

        if (!File.Exists(path))
        {
            return new AppDetail(path, null, null);
        }

        var versionInfo = FileVersionInfo.GetVersionInfo(path);
        var icon = Icon.ExtractAssociatedIcon(path);
        return new AppDetail(path, versionInfo, icon);
    }
}
