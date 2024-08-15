using System;
using System.Windows;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal static class Theme
    {
        private static Boolean _IsDarkMode = false;
        internal static Boolean IsDarkMode
        {
            get => _IsDarkMode;
            set
            {
                _IsDarkMode = value;

                Changed?.Invoke();
                UI.InvokeColorProviderChanged();
            }
        } // update must run on UI thread

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal delegate void ThemeHandler();
        internal static event ThemeHandler Changed;

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal static void SetNonClientArea(Object sender, RoutedEventArgs e) => SetWindowAttributes((Window)sender);

        internal static void UpdateNonClientArea(Window sender) => SetWindowAttributes(sender);

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private static void SetWindowAttributes(Window targetWindow)
        {
            DWMAPI.SetTheme(targetWindow, IsDarkMode);

            if (IsDarkMode)
            {
                DWMAPI.SetCaptionColor(targetWindow, 2105376u); // = rgb 32, 32, 32
            }
            else
            {
                DWMAPI.SetCaptionColor(targetWindow, 15987699u); // = rgb 243, 243, 243
            }
        }
    }
}