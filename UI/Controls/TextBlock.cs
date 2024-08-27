using System;
using System.Windows;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class TextBlock : System.Windows.Controls.TextBlock
    {
        private Boolean _useGrayScaleRender = true;
        internal Boolean UseGrayScaleRender
        {
            get => _useGrayScaleRender;
            set
            {
                _useGrayScaleRender = value;

                if (value)
                {
                    SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
                }
                else
                {
                    SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
                }
            }
        }

        internal enum TextTypes
        {
            Normal = 0,
            Background = 1,
            Disabled = 2,
        }
        private TextTypes _textType = TextTypes.Normal;
        internal TextTypes TextType
        {
            get => _textType;
            set
            {
                _textType = value;

                UpdateTextColor();
            }
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        public TextBlock() => Initialize(true);
        public TextBlock(Boolean useInternalThemeChangedEvent) => Initialize(useInternalThemeChangedEvent);

        private void Initialize(Boolean useInternalThemeChangedEvent)
        {
            FontSize = 14d;
            FontFamily = Fonts.Inter;
            SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
            SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Enabled);
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            if (useInternalThemeChangedEvent)
            {
                UpdateTextColor();

                Theme.Changed += UpdateTextColor;
            }
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private static class Colors
        {
            internal static class DarkMode
            {
                internal static readonly SolidColorBrush Normal = Brushes.White;
                internal static readonly SolidColorBrush Background = new(Color.FromRgb(0xd0, 0xd0, 0xd0));
                internal static readonly SolidColorBrush Disabled = new(Color.FromRgb(0x78, 0x78, 0x78));
            }

            internal static class LightMode
            {
                internal static readonly SolidColorBrush Normal = new(Color.FromRgb(0x1a, 0x1a, 0x1a));
                internal static readonly SolidColorBrush Background = new(Color.FromRgb(0x5e, 0x5e, 0x5e));
                internal static readonly SolidColorBrush Disabled = new(Color.FromRgb(0xa1, 0xa1, 0xa1));
            }
        }

        private readonly SolidColorBrushAnimation _fontColorAnimation = new() { Duration = UI.LongAnimationDuration };

        private void UpdateTextColor()
        {
            _fontColorAnimation.From = (SolidColorBrush)Foreground;
            _fontColorAnimation.To = _textType switch
            {
                TextTypes.Normal => Theme.IsDarkMode ? Colors.DarkMode.Normal : Colors.LightMode.Normal,
                TextTypes.Background => Theme.IsDarkMode ? Colors.DarkMode.Background : Colors.LightMode.Background,
                TextTypes.Disabled => Theme.IsDarkMode ? Colors.DarkMode.Disabled : Colors.LightMode.Disabled,
                _ => Theme.IsDarkMode ? Colors.DarkMode.Normal : Colors.LightMode.Normal
            };

            BeginAnimation(ForegroundProperty, _fontColorAnimation);
        }
    }
}