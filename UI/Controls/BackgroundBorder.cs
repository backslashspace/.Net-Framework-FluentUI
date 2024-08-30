using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class BackgroundBorder : Border
    {
        new internal UIElement Child
        {
            get => _background.Child;
            set => _background.Child = value;
        }

        new internal CornerRadius CornerRadius
        {
            get => base.CornerRadius;
            set
            {
                base.CornerRadius = value;

                Double topLeft = value.TopLeft > 0 ? value.TopLeft - 1 : 0;
                Double topRight = value.TopRight > 0 ? value.TopRight - 1 : 0;
                Double bottomRight = value.BottomRight > 0 ? value.BottomRight - 1 : 0;
                Double bottomLeft = value.BottomLeft > 0 ? value.BottomLeft - 1 : 0;

                _background.CornerRadius = new(topLeft, topRight, bottomRight, bottomLeft);
            }
        }

        private readonly Border _background = new();

        //

        public BackgroundBorder()
        {
            base.CornerRadius = new(5d);
            base.Child = _background;

            _background.CornerRadius = new(4d);
            _background.Margin = new(1d);

            Theme_Changed();

            Theme.Changed += Theme_Changed;

            Loaded += (s, e) => CornerRadius = base.CornerRadius;
        }

        //

        private static readonly SolidColorBrush _darkColorBorder = new(Color.FromRgb(0x1d, 0x1d, 0x1d));
        private static readonly SolidColorBrush _darkColorBackground = new(Color.FromRgb(0x2b, 0x2b, 0x2b));

        private static readonly SolidColorBrush _lightColorBorder = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
        private static readonly SolidColorBrush _lightColorBackground = new(Color.FromRgb(0xfb, 0xfb, 0xfb));

        private void Theme_Changed()
        {
            if (Theme.IsDarkMode)
            {
                Background = _darkColorBorder;
                _background.Background = _darkColorBackground;
            }
            else
            {
                Background = _lightColorBorder;
                _background.Background = _lightColorBackground;
            }
        }
    }
}