using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal sealed class Button_Primary : Border
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

                if (IsEnabled)
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

        internal delegate void ClickHandler(Button_Primary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;

        public Button_Primary()
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
            _TextBlock.Foreground = Brushes.Black;
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

        private readonly SolidColorBrushAnimation _MouseOver_Border_Animation = new() { Duration = _LongDuration };
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
            _MouseOver_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _MouseOver_Border_Animation);

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

            _MouseOver_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _MouseOver_Border_Animation);

            _MouseOver_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _MouseOver_Background_Animation);

            _Idle_Font_Animation.From = (SolidColorBrush)_TextBlock.Foreground;
            _TextBlock.BeginAnimation(TextBlock.ForegroundProperty, _Idle_Font_Animation);

            _MouseUpPending = false;

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _Idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _Idle_Border_Animation);

            _Idle_Background_Animation.From = (SolidColorBrush)_InnerBorder.Background;
            _InnerBorder.BeginAnimation(BackgroundProperty, _Idle_Background_Animation);

            if (_MouseUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
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

        private static readonly SolidColorBrush _DarkMode_FontColor_MouseDown = new(Color.FromRgb(0x2d, 0x2d, 0x2d));
        private static readonly SolidColorBrush _DarkMode_FontColor_Disabled = new(Color.FromRgb(0xa7, 0xa7, 0xa7));
        private static readonly SolidColorBrush _DarkMode_BorderAndBackground_Disabled = new(Color.FromRgb(0x43, 0x43, 0x43));

        private static readonly SolidColorBrush _LightMode_FontColor_MouseDown = new(Color.FromRgb(0xeb, 0xeb, 0xeb));
        private static readonly SolidColorBrush _LightMode_BorderAndBackground_Disabled = new(Color.FromRgb(0xbf, 0xbf, 0xbf));

        private void ColorProviderChanged()
        {
            if (Theme.IsDarkMode)
            {
                Background = AccentColors.DarkMode.BorderColorAsBrush;
                _InnerBorder.Background = AccentColors.DarkMode.PrimaryColorAsBrush;
                _TextBlock.Foreground = Brushes.Black;

                //

                _Idle_Font_Animation.To = Brushes.Black;
                _Idle_Border_Animation.To = AccentColors.DarkMode.BorderColorAsBrush;
                _Idle_Background_Animation.To = AccentColors.DarkMode.PrimaryColorAsBrush;

                _MouseOver_Border_Animation.To = AccentColors.DarkMode.BorderColorMouseOverAsBrush;
                _MouseOver_Background_Animation.To = AccentColors.DarkMode.MouseOverAsBrush;

                _MouseDown_Font_Animation.To = _DarkMode_FontColor_MouseDown;
                _MouseDown_Border_Animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _MouseDown_Background_Animation.To = AccentColors.DarkMode.MouseDownAsBrush;

                _Disable_Font_Animation.To = _DarkMode_FontColor_Disabled;
                _Disable_Border_Animation.To = _DarkMode_BorderAndBackground_Disabled;
                _Disable_Background_Animation.To = _DarkMode_BorderAndBackground_Disabled;
            }
            else
            {
                Background = AccentColors.LightMode.BorderColorAsBrush;
                _InnerBorder.Background = AccentColors.LightMode.PrimaryColorAsBrush;
                _TextBlock.Foreground = Brushes.White;

                //

                _Idle_Font_Animation.To = Brushes.White;
                _Idle_Border_Animation.To = AccentColors.LightMode.BorderColorAsBrush;
                _Idle_Background_Animation.To = AccentColors.LightMode.PrimaryColorAsBrush;

                _MouseOver_Border_Animation.To = AccentColors.LightMode.BorderColorMouseOverAsBrush;
                _MouseOver_Background_Animation.To = AccentColors.LightMode.MouseOverAsBrush;

                _MouseDown_Font_Animation.To = _LightMode_FontColor_MouseDown;
                _MouseDown_Border_Animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _MouseDown_Background_Animation.To = AccentColors.LightMode.MouseDownAsBrush;

                _Disable_Font_Animation.To = Brushes.White;
                _Disable_Border_Animation.To = _LightMode_BorderAndBackground_Disabled;
                _Disable_Background_Animation.To = _LightMode_BorderAndBackground_Disabled;
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