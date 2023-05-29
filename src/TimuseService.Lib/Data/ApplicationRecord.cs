namespace TimuseService;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ApplicationRecord
{
    // 63.....................................39.........................15........0
    //  | timestamp(seconds from 00:00:00 UTC) | timespan(centi-seconds)  | app id |
    public ulong Data;

    public ushort ApplicationId
    {
        get => (ushort)(Data & 0xFFFF);
        set => Data = Data & 0xFFFFFF_FFFFFF_0000 | value;
    }

    public int TimeSpan
    {
        get => (int)((Data & 0x000000_FFFFFF_0000) >> 16);
        set => Data = Data & 0xFFFFFF_000000_FFFF | ((ulong)value << 16);
    }

    public int TimeStamp
    {
        get => (int)((Data & 0xFFFFFF_000000_0000) >> 40);
        set => Data = Data & 0x000000_FFFFFF_FFFF | ((ulong)value << 40);
    }
}
