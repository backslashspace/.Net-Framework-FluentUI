using System;

namespace FluentUI_Framework
{
    internal static class Entry
    {
        [STAThread()]
        internal static void Main()
        {
            App app = new();

            FluentUI.UI.Initialize(app.Dispatcher);

            UI.Dispatcher = app.Dispatcher;
            
            try
            {
                app.InitializeComponent();
                app.Run();
            }
            catch { }

            FluentUI.RegistryWatcher.Dispose();
        }
    }
}