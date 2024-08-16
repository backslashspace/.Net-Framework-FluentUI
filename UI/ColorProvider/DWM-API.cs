using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace FluentUI
{
    internal static class DWMAPI
    {
        internal static Boolean Initialized { get; private set; } = false;

        internal static void Initialize()
        {
            if (Initialized) return;

            GetWindowsBuildNumber();

            if (_windowsBuildNumber >= 18985) // windows 10 '20H1' or newer
            {
                _internalThemeAttribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;
                DarkModeCompatibilityLevel = DWM_Dark_Mode_Compatibility_Level.IMMERSIVE_DARK_MODE;
            }
            else if (_windowsBuildNumber >= 17763)
            {
                _internalThemeAttribute = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_18985_EQUAL_OR_AFTER_17763;
                DarkModeCompatibilityLevel = DWM_Dark_Mode_Compatibility_Level.IMMERSIVE_DARK_MODE_BEFORE_18985_EQUAL_OR_AFTER_17763;
            }
            else
            {
                _internalThemeAttribute = 0;
                DarkModeCompatibilityLevel = DWM_Dark_Mode_Compatibility_Level.NONE;
            }

            Initialized = true;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # 

        internal static Int32 GetWindowsBuildNumber()
        {
            if (_windowsBuildNumber != -1) return _windowsBuildNumber;

            Object regOutput = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CurrentBuildNumber", null);

            if (regOutput == null)
            {
                return -2;
            }

            if (UInt32.TryParse(unchecked((String)regOutput), out UInt32 version))
            {
                _windowsBuildNumber = checked((Int32)version);

                return _windowsBuildNumber;
            }

            return -3;
        }

        private static IntPtr GetWindowHandle(Window window) => new System.Windows.Interop.WindowInteropHelper(window).Handle;

        // # # # # # # # # # # # # # # # # # # # # # # # # 

        #region State
        internal static DWM_Dark_Mode_Compatibility_Level? DarkModeCompatibilityLevel { get; private set; } = null;
        internal enum DWM_Dark_Mode_Compatibility_Level : Byte
        {
            NONE = 0,
            IMMERSIVE_DARK_MODE_BEFORE_18985_EQUAL_OR_AFTER_17763 = 1,
            IMMERSIVE_DARK_MODE = 2,
        }

        private static DWMWINDOWATTRIBUTE _internalThemeAttribute = 0;
        private enum DWMWINDOWATTRIBUTE : UInt32
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_18985_EQUAL_OR_AFTER_17763 = 19,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_BORDER_COLOR = 34,
            DWMWA_CAPTION_COLOR = 35,
            DWMWA_TEXT_COLOR = 36
        }

        private static Int32 _windowsBuildNumber = -1;
        #endregion

        #region Call Wrapper
        // requires os version 17763 or later  
        internal static unsafe Boolean SetTheme(Window window, Boolean dark)
        {
            if (!Initialized) throw new InvalidOperationException("DWMAPI not initialized.");
            if (DarkModeCompatibilityLevel == DWM_Dark_Mode_Compatibility_Level.NONE) return false; // not supported

            DwmSetWindowAttribute(GetWindowHandle(window), _internalThemeAttribute, (UInt32*)&dark, sizeof(UInt32));

            if (_windowsBuildNumber < 22000) UpdateWindow(window);

            return true;
        }

        // requires os version 22000 or later  
        internal static unsafe Boolean SetCaptionColor(Window window, UInt32 COLORREF)
        {
            if (!Initialized) throw new InvalidOperationException("DWMAPI not initialized.");
            if (_windowsBuildNumber < 22000) return false; // not supported

            DwmSetWindowAttribute(GetWindowHandle(window), DWMWINDOWATTRIBUTE.DWMWA_CAPTION_COLOR, &COLORREF, sizeof(UInt32));

            return true;
        }

        // requires os version 22000 or later  
        internal static unsafe Boolean SetBorderColor(Window window, UInt32 COLORREF)
        {
            if (!Initialized) throw new InvalidOperationException("DWMAPI not initialized.");
            if (_windowsBuildNumber < 22000) return false; // not supported

            DwmSetWindowAttribute(GetWindowHandle(window), DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, &COLORREF, sizeof(UInt32));

            return true;
        }
        #endregion

        // # # # # # # # # # # # # # # # # # # # # # # # # 

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static unsafe extern Int32 DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, UInt32* pvAttribute, UInt32 cbAttribute);

        // # # # # # # # # # # # # # # # # # # # # # # # # 

        #region Windows 10 Window Helper
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
        #endregion
    }
}