using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Timuse.UI.DataContexts;

internal class IndexLoader
{
    private readonly string filePath;
    private readonly long readEnd;
    private uint startDays;
    private uint[] indexArray;

    internal IndexLoader(string filePath)
    {
        this.filePath = filePath;
        Load();
    }

    internal void Load()
    {
        var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
        var reader = new BinaryReader(stream, Encoding.UTF8);
        startDays = reader.ReadUInt32();
        var indexCount = reader.BaseStream.Length / Marshal.SizeOf<uint>();
        var i = 0;
        var indexArray = new uint[indexCount - 1];
        while (stream.Position < stream.Length)
        {
            indexArray[i] = reader.ReadUInt32();
            i += 1;
        }

        this.indexArray = indexArray;
    }

    public long FindRecordIndex(uint days)
    {
        if (days <= startDays)
        {
            return 0;
        }
        else if (days > startDays + indexArray.Length)
        {
            return -1;
        }
        else
        {
            return indexArray[days - startDays - 1];
        }
    }
}
