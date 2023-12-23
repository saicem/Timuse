using Microsoft.UI.Xaml;

namespace Teanuts.Extension;

internal static class WindowExtension
{
    public static bool TrySetMicaBackdrop(this Window window, bool useMicaAlt)
    {
        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            Microsoft.UI.Xaml.Media.MicaBackdrop micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
            micaBackdrop.Kind = useMicaAlt ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
            window.SystemBackdrop = micaBackdrop;

            return true; // Succeeded.
        }

        return false; // Mica is not supported on this system.
    }

    public static bool TrySetDesktopAcrylicBackdrop(this Window window)
    {
        if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
        {
            Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop desktopAcrylicBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
            window.SystemBackdrop = desktopAcrylicBackdrop;

            return true; // Succeeded.
        }

        return false; // DesktopAcrylic is not supported on this system.
    }
}
