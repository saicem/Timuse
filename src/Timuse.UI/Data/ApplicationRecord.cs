namespace Timuse.UI.Data;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ApplicationRecord
{
    /// <summary>
    /// Atom unit for the time record
    /// </summary>
    public const long TTick = TimeSpan.TicksPerMillisecond * 10;

    // 63.........................................39.........................15......0
    //  | RecordStartAt(TTicks from 00:00:00 UTC) | Duration(centi-seconds)  | AppId |
    public ulong Data;

    public ushort ApplicationId
    {
        get => (ushort)(this.Data & 0xFFFF);
        set => this.Data = (this.Data & 0xFFFFFF_FFFFFF_0000) | value;
    }

    public int DurationTTicks
    {
        get => (int)((this.Data & 0x000000_FFFFFF_0000) >> 16);
        set => this.Data = (this.Data & 0xFFFFFF_000000_FFFF) | ((ulong)value << 16);
    }

    public int RecordStartAtTTicks
    {
        get => (int)((this.Data & 0xFFFFFF_000000_0000) >> 40);
        set => this.Data = (this.Data & 0x000000_FFFFFF_FFFF) | ((ulong)value << 40);
    }

    public TimeSpan Duration
    {
        get => TTicksToTimeSpan(this.DurationTTicks);
        set => this.DurationTTicks = TimeSpanToTTicks(value);
    }

    public TimeSpan RecordStartAt
    {
        get => TTicksToTimeSpan(this.RecordStartAtTTicks);
        set => this.RecordStartAtTTicks = TimeSpanToTTicks(value);
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
