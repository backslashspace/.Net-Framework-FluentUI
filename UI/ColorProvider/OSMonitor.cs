using System;
using System.Diagnostics;
using System.Management;

#pragma warning disable IDE0079
#pragma warning disable IDE0052
#pragma warning disable CS8618
#pragma warning disable CS8625

namespace FluentUI
{
    internal static class OSMonitor
    {
        internal static Boolean Initialized { get; private set; } = false;

        internal static void Initialize()
        {
            if (Initialized) return;

            StartAccentColorWatcher();
            StartThemeWatcher();

            Initialized = true;
        }

        #region AccentColor
        private static RegistryWatcher _registryAccentWatcher;
        private static void StartAccentColorWatcher()
        {
            if (DWMAPI.GetWindowsBuildNumber() <= 9600) return;

            UpdateAccentColor(null, null);

            _registryAccentWatcher = new RegistryWatcher(@"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Accent", "AccentPalette", UpdateAccentColor);

            Trace.Assert(_registryAccentWatcher.SuccessfullySubscribed, "Unable to start RegistryWatcher (accent color updater)\n\nran out of wmi space?ran out of wmi space? It is safe to continue, accent colors just won't dynamically update in runtime");
        }

        private static void UpdateAccentColor(Object sender, EventArrivedEventArgs e)
        {
            try
            {
                Object rawAccentPalette = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Accent", "AccentPalette", null);
                if (rawAccentPalette == null || rawAccentPalette is not Byte[]) return;
                
                Byte[] accentPalette = (Byte[])rawAccentPalette;
                if (accentPalette.Length != 32) return;

                Boolean colorHasChanged = false;

                for (UInt16 i = 0; i < 32; ++i)
                {
                    if (accentPalette[i] != AccentColors.AccentPalette[i])
                    {
                        colorHasChanged = true;
                        break;
                    }
                }

                if (colorHasChanged)
                {
                    Buffer.BlockCopy(accentPalette, 0, AccentColors.AccentPalette, 0, 32);

                    UI.Dispatcher.Invoke(() =>
                    {
                        AccentColors.CalculateDerivedColors(); // must run on ui thread

                        AccentColors.InvokeChanged();
                        UI.InvokeColorProviderChanged();
                    });
                }
            }
            catch { }
        }
        #endregion

        #region Theme
        private static RegistryWatcher _registryThemeWatcher;
        private static void StartThemeWatcher()
        {
            if (DWMAPI.DarkModeCompatibilityLevel == DWMAPI.DWM_Dark_Mode_Compatibility_Level.NONE) return;

            UpdateTheme(null, null);

            _registryThemeWatcher = new RegistryWatcher(@"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", UpdateTheme);

            Trace.Assert(_registryThemeWatcher.SuccessfullySubscribed, "Unable to start RegistryWatcher (theme color updater)\n\nran out of wmi space? It is safe to continue, the theme just won't dynamically update in runtime");
        }

        private static void UpdateTheme(Object sender, EventArrivedEventArgs e)
        {
            try
            {
                Object rawAppsUseLightTheme = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", null);
                if (rawAppsUseLightTheme == null) return;

                Boolean newValue;
                Int32 appsUseLightTheme = (Int32)rawAppsUseLightTheme;
                
                switch (appsUseLightTheme)
                {
                    case 0:
                        newValue = true;
                        break;

                    case 1:
                        newValue = false;
                        break;

                    default: 
                        return;
                }

                if (Theme.IsDarkMode != newValue)
                {
                    UI.Dispatcher.Invoke(() =>
                    {
                        Theme.IsDarkMode = newValue;
                        UI.InvokeColorProviderChanged();
                    });
                }
            }
            catch { }
        }
        #endregion
    }
}