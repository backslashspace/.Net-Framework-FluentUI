using System;
using System.Windows;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal static class Theme
    {
        internal static Boolean IsDarkMode = false;

        // # # # # # # # # # # # # # # # # # # # # # # # # # #
        
        internal delegate void ThemeHandler();
        internal static event ThemeHandler Changed;
        internal static ThemeHandler GetChangedInvoker() => Changed!;

        // # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal static void SetNonClientArea(Object sender, RoutedEventArgs e)
        {
            DWMAPI.SetTheme(new System.Windows.Interop.WindowInteropHelper((Window)sender).Handle, IsDarkMode);

            if (DWMAPI.GetWindowsBuildNumber() < 22000)
            {
                UpdateWindow((Window)sender);
            }
        }

        internal static void UpdateNonClientArea(Window sender)
        {
            DWMAPI.SetTheme(new System.Windows.Interop.WindowInteropHelper(sender).Handle, IsDarkMode);

            if (DWMAPI.GetWindowsBuildNumber() < 22000)
            {
                UpdateWindow(sender);
            }
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # #

        private static void UpdateWindow(Window window)
        {
            if (window.WindowStyle != WindowStyle.ToolWindow && window.WindowStyle != WindowStyle.None)
            {
                WindowStyle current = window.WindowStyle;

                window.WindowStyle = current switch
                {
                    WindowStyle.SingleBorderWindow => WindowStyle.ThreeDBorderWindow,
                    WindowStyle.ThreeDBorderWindow => WindowStyle.SingleBorderWindow,
                    WindowStyle.ToolWindow => WindowStyle.SingleBorderWindow,
                    _ => current,
                };

                window.WindowStyle = current;
            }
            else
            {
                ResizeMode current = window.ResizeMode;

                window.ResizeMode = current switch
                {
                    ResizeMode.CanResize => ResizeMode.CanMinimize,
                    ResizeMode.NoResize => ResizeMode.CanMinimize,
                    _ => ResizeMode.CanResize
                };

                window.ResizeMode = current;
            }
        }
    }
}