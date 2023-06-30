using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using TimuseWinUI.Controls;
using TimuseWinUI.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimuseWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Closed += MainWindow_Closed;

            // todo get this
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(titleBar);
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
        }

        private TabButton lastActiveTab = null;

        private void NavigatorTabTapped(object sender, TappedRoutedEventArgs e)
        {
            var activeTab = (sender as TabButton);
            if (!activeTab.Active)
            {
                navigate(activeTab);
                if (lastActiveTab != null)
                {
                    lastActiveTab.Active = false;
                }
                activeTab.Active = true;
                lastActiveTab = activeTab;
            }
        }

        private void navigate(TabButton tab)
        {
            if (tab == generalTab)
            {
                frame.Navigate(typeof(GeneralPage));
            }
            else if (tab == statisticTab)
            {
                frame.Navigate(typeof(StatisticPage));
            }
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
    }
}
