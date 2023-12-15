using System;
using System.Runtime.InteropServices;

namespace Timuse.UI.Models;

[StructLayout(LayoutKind.Sequential)]
public struct AppRecord
{
    /// <summary>
    /// Atom unit for the time record.
    /// </summary>
    public const long TTick = TimeSpan.TicksPerMillisecond * 10;

    // 63.........................................39.........................15......0
    //  | RecordStartAt(TTicks from 00:00:00 UTC) | Duration(centi-seconds)  | AppId |
    public ulong Data;

    public ushort AppId
    {
        get => (ushort)(Data & 0xFFFF);
        set => Data = (Data & 0xFFFFFF_FFFFFF_0000) | value;
    }

    public int DurationTTicks
    {
        get => (int)((Data & 0x000000_FFFFFF_0000) >> 16);
        set => Data = (Data & 0xFFFFFF_000000_FFFF) | ((ulong)value << 16);
    }

    public int StartAtTTicks
    {
        get => (int)((Data & 0xFFFFFF_000000_0000) >> 40);
        set => Data = (Data & 0x000000_FFFFFF_FFFF) | ((ulong)value << 40);
    }

    public long DurationTicks
    {
        get => DurationTTicks * TTick;
        set => DurationTTicks = (int)(value / TTick);
    }

    public long StartAtTicks
    {
        get => StartAtTTicks * TTick;
        set => StartAtTTicks = (int)(value / TTick);
    }

    public TimeSpan Duration
    {
        get => TimeSpan.FromTicks(DurationTicks);
        set => DurationTicks = value.Ticks;
    }

    public TimeSpan StartAt
    {
        get => TimeSpan.FromTicks(DurationTicks);
        set => StartAtTicks = value.Ticks;
    }
}
