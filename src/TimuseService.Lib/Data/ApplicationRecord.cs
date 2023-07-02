namespace TimuseService;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ApplicationRecord
{
    public const long TTick = TimeSpan.TicksPerMillisecond * 10;

    // 63.....................................39.........................15........0
    //  | timestamp(seconds from 00:00:00 UTC) | timespan(centi-seconds)  | app id |
    public ulong Data;

    public ushort ApplicationId
    {
        get => (ushort)(Data & 0xFFFF);
        set => Data = Data & 0xFFFFFF_FFFFFF_0000 | value;
    }

    public int DurationTTicks
    {
        get => (int)((Data & 0x000000_FFFFFF_0000) >> 16);
        set => Data = Data & 0xFFFFFF_000000_FFFF | ((ulong)value << 16);
    }

    public int RecordStartAtTTicks
    {
        get => (int)((Data & 0xFFFFFF_000000_0000) >> 40);
        set => Data = Data & 0x000000_FFFFFF_FFFF | ((ulong)value << 40);
    }

    public TimeSpan Duration
    {
        get => TTicksToTimeSpan(DurationTTicks);
        set => DurationTTicks = TimeSpanToTTicks(value);
    }

    public TimeSpan RecordStartAt
    {
        get => TTicksToTimeSpan(RecordStartAtTTicks);
        set => RecordStartAtTTicks = TimeSpanToTTicks(value);
    }

    public static TimeSpan TTicksToTimeSpan(int tticks)
    {
        return TimeSpan.FromTicks(TTick * tticks);
    }

    public static int TimeSpanToTTicks(TimeSpan timeSpan)
    {
        return (int)(timeSpan.Ticks / TTick);
    }
}
