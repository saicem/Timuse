using System.Collections.Generic;
using System.IO;
using System.Text;
using Timuse.UI.Models;

namespace Timuse.UI.DataContexts;

internal class RecordLoader
{
    private readonly BinaryReader reader;

    public RecordLoader(string filePath)
    {
        var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
        reader = new BinaryReader(stream, Encoding.UTF8);
    }

    public List<AppRecord> GetRecords(long startPosition, long endPosition)
    {
        if (startPosition == -1)
        {
            return new List<AppRecord>();
        }

        if (endPosition == -1)
        {
            endPosition = reader.BaseStream.Length;
        }

        var ret = new List<AppRecord>();
        reader.BaseStream.Position = startPosition;
        while (reader.BaseStream.Position < endPosition)
        {
            ret.Add(new AppRecord { Data = reader.ReadUInt64() });
        }

        return ret;
    }
}
