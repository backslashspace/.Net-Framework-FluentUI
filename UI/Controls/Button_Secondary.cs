using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal sealed class Button_Secondary : Border
    {
        private String _content;
        internal String Content
        {
            get => _content;
            set
            {
                _content = value;
                _textBlock.Text = value;
            }
        }

        private Boolean _isEnabled = true;
        new internal Boolean IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;

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

        private readonly Border _this;
        private readonly Border _innerBorder = new();
        private readonly TextBlock _textBlock = new();

        internal delegate void ClickHandler(Button_Secondary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;

        public Button_Secondary()
        {
            _this = this;
            UseLayoutRounding = true;
            CornerRadius = new(5d);
            Focusable = true;
            Height = 32d;
            Width = 152d;
            Child = _innerBorder;

            _innerBorder.Margin = new(1d);
            _innerBorder.CornerRadius = new(4d);
            _innerBorder.Child = _textBlock;

            _textBlock.FontSize = 14d;
            _textBlock.FontFamily = Fonts.Inter;
            _textBlock.FontWeight = FontWeights.Medium;
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            _textBlock.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.Grayscale);
            _textBlock.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            _textBlock.SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Enabled);
            _textBlock.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);

            if (Theme.IsDarkMode)
            {
                Background = Colors.DarkMode.Border;
                _innerBorder.Background = Colors.DarkMode.Background;
                _textBlock.Foreground = Brushes.White;
            }
            else
            {
                Background = Colors.LightMode.Border;
                _innerBorder.Background = Colors.LightMode.Background;
                _textBlock.Foreground = Colors.LightMode.Font;
            }

            MouseEnter += MouseEnterHandler;
            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            MouseLeave += MouseLeaveHandler;

            UI.ColorProviderChanged += ColorProviderChanged;

            Loaded += (s, e) =>
            {
                _isEnabled = _this.IsEnabled; // xaml interface not using 'overridden' IsEnabled property
                ColorProviderChanged();
            };
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region BrushAnimations
        private static readonly Duration _shortDuration = new(new(0, 0, 0, 0, 24));
        private static readonly Duration _longDuration = new(new(0, 0, 0, 0, 48));

        private readonly SolidColorBrushAnimation _idle_Font_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_Border_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_Background_Animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _mouseOver_Background_Animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _buttonDown_Font_Animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _buttonDown_Border_Animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _buttonDown_Background_Animation = new() { Duration = _shortDuration };

        private readonly SolidColorBrushAnimation _disable_Font_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_Border_Animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_Background_Animation = new() { Duration = _longDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _buttonDown_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _buttonDown_Border_Animation);

            _buttonDown_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _buttonDown_Background_Animation);

            _buttonDown_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _buttonDown_Font_Animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_Border_Animation);

            _mouseOver_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _mouseOver_Background_Animation);

            _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);

            _buttonUpPending = false;
        }
        #endregion

        //

        #region KeyBoardHandler
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;
            if (e.OriginalSource != this) return;

            BeginButtonDownAnimation();

            e.Handled = true;
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.Space) return;

            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            e.Handled = true;
            Click?.Invoke(this);
        }
        #endregion

        #region MouseHandler
        private void MouseEnterHandler(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseOver_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _mouseOver_Background_Animation);
        }

        private void PreviewMouseDownHandler(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginButtonDownAnimation();
        }

        private void PreviewMouseUpHandler(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _idle_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _idle_Background_Animation);

            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _idle_Border_Animation);

                _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);

                _buttonUpPending = false;
            }
        }
        #endregion

        #region En/Disable
        private void Disable()
        {
            MouseLeave -= MouseLeaveHandler;
            _this.IsEnabled = false;

            //

            _disable_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _disable_Border_Animation);

            _disable_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _disable_Background_Animation);

            _disable_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_Font_Animation);
        }

        private void Enable()
        {
            _idle_Border_Animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_Border_Animation);

            _idle_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
            _innerBorder.BeginAnimation(BackgroundProperty, _idle_Background_Animation);

            _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);

            _this.IsEnabled = true;
            MouseLeave += MouseLeaveHandler;
        }
        #endregion

        //

        private static class Colors
        {
            internal static class LightMode
            {
                internal static readonly SolidColorBrush FocusVisual = new(Color.FromRgb(0x1a, 0x1a, 0x1a));

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
            FrameworkElementFactory focusVisualFrameworkElementFactory = new(typeof(Border));
            focusVisualFrameworkElementFactory.SetValue(BorderThicknessProperty, new Thickness(2d));
            focusVisualFrameworkElementFactory.SetValue(MarginProperty, new Thickness(-3d));
            focusVisualFrameworkElementFactory.SetValue(CornerRadiusProperty, new CornerRadius(7.5d));

            if (Theme.IsDarkMode)
            {
                focusVisualFrameworkElementFactory.SetValue(BorderBrushProperty, Brushes.White);

                _idle_Font_Animation.To = Brushes.White;
                _idle_Border_Animation.To = Colors.DarkMode.Border;
                _idle_Background_Animation.To = Colors.DarkMode.Background;

                _mouseOver_Background_Animation.To = Colors.DarkMode.BackgroundMouseOver;

                _buttonDown_Font_Animation.To = Colors.DarkMode.FontMouseDown;
                _buttonDown_Border_Animation.To = Colors.DarkMode.BorderMouseDown;
                _buttonDown_Background_Animation.To = Colors.DarkMode.BackgroundMouseDown;

                _disable_Font_Animation.To = Colors.DarkMode.FontDisabled;
                _disable_Border_Animation.To = Colors.DarkMode.BorderDisabled;
                _disable_Background_Animation.To = Colors.DarkMode.BackgroundDisabled;
            }
            else
            {
                focusVisualFrameworkElementFactory.SetValue(BorderBrushProperty, Colors.LightMode.FocusVisual);

                _idle_Font_Animation.To = Colors.LightMode.Font;
                _idle_Border_Animation.To = Colors.LightMode.Border;
                _idle_Background_Animation.To = Colors.LightMode.Background;

                _mouseOver_Background_Animation.To = Colors.LightMode.BackgroundMouseOver;

                _buttonDown_Font_Animation.To = Colors.LightMode.FontMouseDown;
                _buttonDown_Border_Animation.To = Colors.LightMode.BorderMouseDown;
                _buttonDown_Background_Animation.To = Colors.LightMode.BackgroundMouseDown;

                _disable_Font_Animation.To = Colors.LightMode.FontDisabled;
                _disable_Border_Animation.To = Colors.LightMode.BorderDisabled;
                _disable_Background_Animation.To = Colors.LightMode.BackgroundDisabled;
            }

            ControlTemplate focusVisualControlTemplate = new(typeof(Control));
            focusVisualControlTemplate.VisualTree = focusVisualFrameworkElementFactory;
            Style style = new();
            style.Setters.Add(new Setter(Control.TemplateProperty, focusVisualControlTemplate));
            FocusVisualStyle = style;

            if (_isEnabled)
            {
                _idle_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _idle_Border_Animation);

                _idle_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
                _innerBorder.BeginAnimation(BackgroundProperty, _idle_Background_Animation);

                _idle_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_Font_Animation);
            }
            else
            {
                _disable_Border_Animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _disable_Border_Animation);

                _disable_Background_Animation.From = (SolidColorBrush)_innerBorder.Background;
                _innerBorder.BeginAnimation(BackgroundProperty, _disable_Background_Animation);

                _disable_Font_Animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_Font_Animation);
            }
        }
    }
}