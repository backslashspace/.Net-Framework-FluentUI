using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal sealed class Button_Secondary : Border
    {
        private String _Content;
        internal String Content
        {
            get => _Content;
            set
            {
                _Content = value;
                _TextBlock.Text = value;
            }
        }

        private Boolean _IsEnabled = true;
        new internal Boolean IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;

                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        private readonly Border _This;
        private readonly Border _InnerBorder = new();
        private readonly TextBlock _TextBlock = new();

        internal delegate void ClickHandler(Button_Secondary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;

        public Button_Secondary()
        {
            _This = this;
            UseLayoutRounding = true;
            CornerRadius = new(5);
            Child = _InnerBorder;

            _InnerBorder.Margin = new(1);
            _InnerBorder.CornerRadius = new(4);
            _InnerBorder.Child = _TextBlock;

            _TextBlock.FontSize = 14;
            _TextBlock.FontFamily = Fonts.Inter;
            _TextBlock.FontWeight = FontWeights.Medium;
            _TextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _TextBlock.VerticalAlignment = VerticalAlignment.Center;
            _TextBlock.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
            _TextBlock.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            _TextBlock.SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Enabled);
            _TextBlock.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            Loaded += (s, e) =>
            {
                _IsEnabled = _This.IsEnabled; // xaml interface not using 'overridden' IsEnabled property
                ColorProviderChanged();
            };

            MouseEnter += MouseEnterHandler;
            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            MouseLeave += MouseLeaveHandler;

            UI.ColorProviderChanged += ColorProviderChanged;
        }

        private static readonly Duration _ShortDuration = new(new(0, 0, 0, 0, 24));
        private static readonly Duration _LongDuration = new(new(0, 0, 0, 0, 48));

        #region BrushAnimations
        private readonly SolidColorBrushAnimation _Idle_Font_Animation = new() { Duration = _LongDuration };
        private readonly SolidColorBrushAnimation _Idle_Border_Animation = new() { Duration = _LongDuration };
        private readonly SolidColorBrushAnimation _Idle_Background_Animation = new() { Duration = _LongDuration };

        private readonly SolidColorBrushAnimation _MouseOver_Background_Animation = new() { Duration = _LongDuration };

        private readonly SolidColorBrushAnimation _MouseDown_Font_Animation = new() { Duration = _ShortDuration };
        private readonly SolidColorBrushAnimation _MouseDown_Border_Animation = new() { Duration = _ShortDuration };
        private readonly SolidColorBrushAnimation _MouseDown_Background_Animation = new() { Duration = _ShortDuration };

        private readonly SolidColorBrushAnimation _Disable_Font_Animation = new() { Duration = _LongDuration };
        private readonly SolidColorBrushAnimation _Disable_Border_Animation = new() { Duration = _LongDuration };
        private readonly SolidColorBrushAnimation _Disable_Background_Animation = new() { Duration = _LongDuration };
        #endregion

        private Boolean _MouseUpPending = false;

        #region MouseHandler
        private void MouseEnterHandler(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _MouseOver_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _MouseOver_Background_Animation);
        }

        private void PreviewMouseDownHandler(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _MouseDown_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _MouseDown_Border_Animation);

            _MouseDown_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _MouseDown_Background_Animation);

            _MouseDown_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
            _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _MouseDown_Font_Animation);

            _MouseUpPending = true;
        }

        private void PreviewMouseUpHandler(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PreviewClick?.Invoke();

            _Idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _Idle_Border_Animation);

            _MouseOver_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _MouseOver_Background_Animation);

            _Idle_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
            _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Idle_Font_Animation);

            _MouseUpPending = false;

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _Idle_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _Idle_Background_Animation);

            if (_MouseUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _Idle_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _Idle_Border_Animation);

                _Idle_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
                _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Idle_Font_Animation);

                _MouseUpPending = false;
            }
        }
        #endregion

        #region En/Disable
        private void Disable()
        {
            MouseLeave -= MouseLeaveHandler;
            _This.IsEnabled = false;

            //

            _Disable_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _Disable_Border_Animation);

            _Disable_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _Disable_Background_Animation);

            _Disable_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
            _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Disable_Font_Animation);
        }

        private void Enable()
        {
            _Idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _Idle_Border_Animation);

            _Idle_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _Idle_Background_Animation);

            _Idle_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
            _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Idle_Font_Animation);

            _This.IsEnabled = true;
            MouseLeave += MouseLeaveHandler;
        }
        #endregion

        //

        private static class Colors
        {
            internal static class LightMode
            {
                internal static readonly SolidColorBrush Border = new(Color.FromRgb(0xd9, 0xd9, 0xd9));
                internal static readonly SolidColorBrush BorderMouseDown = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
                internal static readonly SolidColorBrush BorderDisabled = new(Color.FromRgb(0xec, 0xec, 0xec));

                internal static readonly SolidColorBrush Font = new(Color.FromRgb(0x1b, 0x1b, 0x1b));
                internal static readonly SolidColorBrush FontMouseDown = new(Color.FromRgb(0x5d, 0x5d, 0x5d));
                internal static readonly SolidColorBrush FontDisabled = new(Color.FromRgb(0xa0, 0xa0, 0xa0));

                internal static readonly SolidColorBrush Background = new(Color.FromRgb(0xfb, 0xfb, 0xfb));
                internal static readonly SolidColorBrush BackgroundMouseOver = new(Color.FromRgb(0xf6, 0xf6, 0xf6));
                internal static readonly SolidColorBrush BackgroundMouseDown = new(Color.FromRgb(0xf5, 0xf5, 0xf5));
                internal static readonly SolidColorBrush BackgroundDisabled = new(Color.FromRgb(0xfa, 0xfa, 0xfa));
            }

            internal static class DarkMode
            {
                internal static readonly SolidColorBrush Border = new(Color.FromRgb(0x35, 0x35, 0x35));
                internal static readonly SolidColorBrush BorderMouseDown = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush BorderDisabled = new(Color.FromRgb(0x3a, 0x3a, 0x3a));

                internal static readonly SolidColorBrush FontMouseDown = new(Color.FromRgb(0xce, 0xce, 0xce));
                internal static readonly SolidColorBrush FontDisabled = new(Color.FromRgb(0x7e, 0x7e, 0x7e));

                internal static readonly SolidColorBrush Background = new(Color.FromRgb(0x2d, 0x2d, 0x2d));
                internal static readonly SolidColorBrush BackgroundMouseOver = new(Color.FromRgb(0x32, 0x32, 0x32));
                internal static readonly SolidColorBrush BackgroundMouseDown = new(Color.FromRgb(0x27, 0x27, 0x27));
                internal static readonly SolidColorBrush BackgroundDisabled = new(Color.FromRgb(0x34, 0x34, 0x34));
            }
        }

        private void ColorProviderChanged()
        {
            if (Theme.IsDarkMode)
            {
                Background = Colors.DarkMode.Border;
                _InnerBorder.Background = Colors.DarkMode.Background;
                _TextBlock.Foreground = Brushes.White;

                //

                _Idle_Font_Animation.To = Brushes.White;
                _Idle_Border_Animation.To = Colors.DarkMode.Border;
                _Idle_Background_Animation.To = Colors.DarkMode.Background;

                _MouseOver_Background_Animation.To = Colors.DarkMode.BackgroundMouseOver;

                _MouseDown_Font_Animation.To = Colors.DarkMode.FontMouseDown;
                _MouseDown_Border_Animation.To = Colors.DarkMode.BorderMouseDown;
                _MouseDown_Background_Animation.To = Colors.DarkMode.BackgroundMouseDown;

                _Disable_Font_Animation.To = Colors.DarkMode.FontDisabled;
                _Disable_Border_Animation.To = Colors.DarkMode.BorderDisabled;
                _Disable_Background_Animation.To = Colors.DarkMode.BackgroundDisabled;
            }
            else
            {
                Background = Colors.LightMode.Border;
                _InnerBorder.Background = Colors.LightMode.Background;
                _TextBlock.Foreground = Colors.LightMode.Font;

                //

                _Idle_Font_Animation.To = Colors.LightMode.Font;
                _Idle_Border_Animation.To = Colors.LightMode.Border;
                _Idle_Background_Animation.To = Colors.LightMode.Background;

                _MouseOver_Background_Animation.To = Colors.LightMode.BackgroundMouseOver;

                _MouseDown_Font_Animation.To = Colors.LightMode.FontMouseDown;
                _MouseDown_Border_Animation.To = Colors.LightMode.BorderMouseDown;
                _MouseDown_Background_Animation.To = Colors.LightMode.BackgroundMouseDown;

                _Disable_Font_Animation.To = Colors.LightMode.FontDisabled;
                _Disable_Border_Animation.To = Colors.LightMode.BorderDisabled;
                _Disable_Background_Animation.To = Colors.LightMode.BackgroundDisabled;
            }

            if (_IsEnabled)
            {
                _Idle_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _Idle_Border_Animation);

                _Idle_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
                _InnerBorder.BeginAnimation(BackgroundProperty, _Idle_Background_Animation);

                _Idle_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
                _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Idle_Font_Animation);
            }
            else
            {
                _Disable_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _Disable_Border_Animation);

                _Disable_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
                _InnerBorder.BeginAnimation(BackgroundProperty, _Disable_Background_Animation);

                _Disable_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
                _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Disable_Font_Animation);
            }
        }
    }
}