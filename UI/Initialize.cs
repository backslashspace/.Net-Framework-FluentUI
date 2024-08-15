using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal static class UI
    {
        internal static Boolean IsInitialized { get; private set; }
        internal static Dispatcher Dispatcher { get; private set; }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal static void Initialize(Dispatcher dispatcher)
        {
            if (IsInitialized) return;

            Dispatcher = dispatcher;

            ExplicitLoadFontReferences();

            DWMAPI.Initialize();
            OSMonitor.Initialize();

            IsInitialized = true;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal delegate void ColorProviderUpdatedHandler();
        internal static event ColorProviderUpdatedHandler ColorProviderChanged;

        internal static void InvokeColorProviderChanged() => ColorProviderChanged?.Invoke();

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private static void ExplicitLoadFontReferences()
        {
            UInt16 fontCounter = 0;

            foreach (FontFamily fontFamily in System.Windows.Media.Fonts.GetFontFamilies(new Uri("pack://application:,,,/UI/resources/fonts/")))
            {
                switch (fontFamily.Source.Split('#')[1])
                {
                    case "Inter":
                        ++fontCounter;
                        Fonts.Inter = fontFamily;
                        break;

                    case "Inter Display":
                        ++fontCounter;
                        Fonts.Inter_Display = fontFamily;
                        break;
                }
            }

            if (fontCounter > 1) return; 

            if (Fonts.Inter == null)
            {
                Trace.Fail("FluentUI.Initialize.ExplicitLoadFontReferences()\n\nUnable to load Font: Inter");
            }

            if (Fonts.Inter_Display == null)
            {
                Trace.Fail("FluentUI.Initialize.ExplicitLoadFontReferences()\n\nUnable to load Font: Inter Display");
            }

            Environment.Exit(-1);
        }
    }
}