using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;

using WinRT;

namespace Timuse.UI;

public sealed partial class MainWindow : Window
{
    private delegate IntPtr WinProc(IntPtr hWnd, PInvoke.User32.WindowMessage msg, IntPtr wParam, IntPtr lParam);

    private WinProc newWndProc = null;
    private IntPtr oldWndProc = IntPtr.Zero;

    [DllImport("user32")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, PInvoke.User32.WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, PInvoke.User32.WindowMessage msg, IntPtr wParam, IntPtr lParam);

    private void SubClassing()
    {
        var hwnd = this.As<IWindowNative>().WindowHandle;

        this.newWndProc = new WinProc(this.NewWindowProc);
        this.oldWndProc = SetWindowLongPtr(hwnd, PInvoke.User32.WindowLongIndexFlags.GWL_WNDPROC, this.newWndProc);
    }

    private readonly int minWidth = 800;
    private readonly int minHeight = 600;

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public PInvoke.POINT PtReserved;
        public PInvoke.POINT PtMaxSize;
        public PInvoke.POINT PtMaxPosition;
        public PInvoke.POINT PtMinTrackSize;
        public PInvoke.POINT PtMaxTrackSize;
    }

    private IntPtr NewWindowProc(IntPtr hWnd, PInvoke.User32.WindowMessage msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case PInvoke.User32.WindowMessage.WM_GETMINMAXINFO:
                var dpi = PInvoke.User32.GetDpiForWindow(hWnd);
                float scalingFactor = (float)dpi / 96;

                MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.PtMinTrackSize.x = (int)(this.minWidth * scalingFactor);
                minMaxInfo.PtMinTrackSize.y = (int)(this.minHeight * scalingFactor);
                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;
        }

        return CallWindowProc(this.oldWndProc, hWnd, msg, wParam, lParam);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
    internal interface IWindowNative
    {
        IntPtr WindowHandle { get; }
    }
}
