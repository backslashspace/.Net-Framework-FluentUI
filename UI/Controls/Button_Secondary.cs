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

        #region Definitions
        private readonly Border _this;
        private readonly Border _buttonBackground = new();
        private readonly TextBlock _textBlock = new(false);

        internal delegate void ClickHandler(Button_Secondary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;
        #endregion

        new internal Boolean IsInitialized { get; private set; } = false;

        public Button_Secondary()
        {
            _this = this;
            UseLayoutRounding = true;
            CornerRadius = new(5d);
            Focusable = true;
            Height = 32d;
            Width = 152d;
            Child = _buttonBackground;

            _buttonBackground.Margin = new(1d);
            _buttonBackground.CornerRadius = new(4d);
            _buttonBackground.Child = _textBlock;

            _textBlock.FontWeight = FontWeights.Medium;
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;

            if (Theme.IsDarkMode)
            {
                Background = Colors.DarkMode.Border;
                _buttonBackground.Background = Colors.DarkMode.Background;
                _textBlock.Foreground = Brushes.White;
            }
            else
            {
                Background = Colors.LightMode.Border;
                _buttonBackground.Background = Colors.LightMode.Background;
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

                IsInitialized = true;
            };
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region BrushAnimations
        private static readonly Duration _shortDuration = new(new(0, 0, 0, 0, 24));
        private static readonly Duration _longDuration = new(new(0, 0, 0, 0, 48));

        private readonly SolidColorBrushAnimation _idle_font_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_border_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _idle_background_animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _mouse_over_background_animation = new() { Duration = _longDuration };

        private readonly SolidColorBrushAnimation _button_down_font_animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _button_down_border_animation = new() { Duration = _shortDuration };
        private readonly SolidColorBrushAnimation _button_down_background_animation = new() { Duration = _shortDuration };

        private readonly SolidColorBrushAnimation _disable_font_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_border_animation = new() { Duration = _longDuration };
        private readonly SolidColorBrushAnimation _disable_background_animation = new() { Duration = _longDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _button_down_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _button_down_border_animation);

            _button_down_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _button_down_background_animation);

            _button_down_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _button_down_font_animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_border_animation);

            _mouse_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _mouse_over_background_animation);

            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

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
            _mouse_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _mouse_over_background_animation);
        }

        private void PreviewMouseDownHandler(Object sender, System.Windows.Input.MouseButtonEventArgs e) => BeginButtonDownAnimation();

        private void PreviewMouseUpHandler(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, System.Windows.Input.MouseEventArgs e)
        {
            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _idle_background_animation);

            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _idle_border_animation);

                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

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

            _disable_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _disable_border_animation);

            _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _disable_background_animation);

            _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);
        }

        private void Enable()
        {
            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(BackgroundProperty, _idle_border_animation);

            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(BackgroundProperty, _idle_background_animation);

            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

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

                _idle_font_animation.To = Brushes.White;
                _idle_border_animation.To = Colors.DarkMode.Border;
                _idle_background_animation.To = Colors.DarkMode.Background;

                _mouse_over_background_animation.To = Colors.DarkMode.BackgroundMouseOver;

                _button_down_font_animation.To = Colors.DarkMode.FontMouseDown;
                _button_down_border_animation.To = Colors.DarkMode.BorderMouseDown;
                _button_down_background_animation.To = Colors.DarkMode.BackgroundMouseDown;

                _disable_font_animation.To = Colors.DarkMode.FontDisabled;
                _disable_border_animation.To = Colors.DarkMode.BorderDisabled;
                _disable_background_animation.To = Colors.DarkMode.BackgroundDisabled;
            }
            else
            {
                focusVisualFrameworkElementFactory.SetValue(BorderBrushProperty, Colors.LightMode.FocusVisual);

                _idle_font_animation.To = Colors.LightMode.Font;
                _idle_border_animation.To = Colors.LightMode.Border;
                _idle_background_animation.To = Colors.LightMode.Background;

                _mouse_over_background_animation.To = Colors.LightMode.BackgroundMouseOver;

                _button_down_font_animation.To = Colors.LightMode.FontMouseDown;
                _button_down_border_animation.To = Colors.LightMode.BorderMouseDown;
                _button_down_background_animation.To = Colors.LightMode.BackgroundMouseDown;

                _disable_font_animation.To = Colors.LightMode.FontDisabled;
                _disable_border_animation.To = Colors.LightMode.BorderDisabled;
                _disable_background_animation.To = Colors.LightMode.BackgroundDisabled;
            }

            ControlTemplate focusVisualControlTemplate = new(typeof(Control));
            focusVisualControlTemplate.VisualTree = focusVisualFrameworkElementFactory;
            Style style = new();
            style.Setters.Add(new Setter(Control.TemplateProperty, focusVisualControlTemplate));
            FocusVisualStyle = style;

            if (IsInitialized)
            {
                AnimateToNewState();
            }
        }

        private void AnimateToNewState()
        {
            if (_isEnabled)
            {
                _idle_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _idle_border_animation);

                _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(BackgroundProperty, _idle_background_animation);

                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);
            }
            else
            {
                _disable_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(BackgroundProperty, _disable_border_animation);

                _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(BackgroundProperty, _disable_background_animation);

                _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);
            }
        }
    }
}