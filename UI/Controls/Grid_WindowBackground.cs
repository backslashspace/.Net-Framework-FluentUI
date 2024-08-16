using System.Windows.Controls;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class Grid_WindowBackground : Grid
    {
        private static readonly SolidColorBrush _darkColor = new(Color.FromRgb(0x20, 0x20, 0x20));
        private static readonly SolidColorBrush _lightColor = new(Color.FromRgb(0xf3, 0xf3, 0xf3));

        public Grid_WindowBackground()
        {
            Theme_Changed();

            Theme.Changed += Theme_Changed;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private void Theme_Changed()
        {
            if (Theme.IsDarkMode)
            {
                Background = _darkColor;
            }
            else
            {
                Background = _lightColor;
            }
        }
    }
}