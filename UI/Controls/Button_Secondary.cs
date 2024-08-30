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
            UseLayoutRounding = true;
            CornerRadius = new(5d);
            Focusable = true;
            Height = 32d;
            Width = 152d;
            BorderThickness = new(0d, 1d, 0d, 0d);
            Child = _buttonBackground;

            _buttonBackground.Margin = new(1d, 0d, 1d, 1d);
            _buttonBackground.CornerRadius = new(4d);
            _buttonBackground.Child = _textBlock;

            _textBlock.FontWeight = FontWeights.Medium;
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;

            PreviewMouseDown += PreviewMouseDownHandler;
            PreviewMouseUp += PreviewMouseUpHandler;
            MouseLeave += MouseLeaveHandler;

            UI.ColorProviderChanged += ColorProviderChanged;

            Loaded += OnLoaded;
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            _isEnabled = base.IsEnabled; // xaml interface not using 'overridden' IsEnabled property

            if (_isEnabled)
            {
                MouseEnter += MouseEnterHandler;

                if (Theme.IsDarkMode)
                {
                    _textBlock.Foreground = Brushes.White;
                    Background = Colors.DarkMode.IdleBorder;
                    BorderBrush = Colors.DarkMode.IdleBorderSpecial;
                    _buttonBackground.Background = Colors.DarkMode.IdleBackground;
                }
                else
                {
                    _textBlock.Foreground = Colors.LightMode.IdleFont;
                    Background = Colors.LightMode.IdleBorder;
                    BorderBrush = Colors.LightMode.IdleBorderSpecial;
                    _buttonBackground.Background = Colors.LightMode.IdleBackground;
                }
            }
            else
            {
                if (Theme.IsDarkMode)
                {
                    _textBlock.Foreground = Colors.DarkMode.DisabledFont;
                    Background = Colors.DarkMode.DisabledBorder;
                    BorderBrush = Colors.DarkMode.DisabledBorderSpecial;
                    _buttonBackground.Background = Colors.DarkMode.DisabledBackground;
                }
                else
                {
                    _textBlock.Foreground = Colors.LightMode.DisabledFont;
                    Background = Colors.LightMode.DisabledBorder;
                    BorderBrush = Colors.LightMode.DisabledBorderSpecial;
                    _buttonBackground.Background = Colors.LightMode.DisabledBackground;
                }
            }

            ColorProviderChanged();

            IsInitialized = true;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region Animation Definitions
        private readonly SolidColorBrushAnimation _idle_font_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_border_special_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_over_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_down_font_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_border_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_border_special_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_background_animation = new() { Duration = UI.ShortAnimationDuration };

        private readonly SolidColorBrushAnimation _disable_font_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_border_special_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_background_animation = new() { Duration = UI.LongAnimationDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _mouse_down_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _mouse_down_font_animation);

            _mouse_down_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _mouse_down_border_animation);

            _mouse_down_border_special_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _mouse_down_border_special_animation);

            _mouse_down_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _mouse_down_background_animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

            _idle_border_special_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _idle_border_special_animation);

            _mouse_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);

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
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);
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
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

                _idle_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

                _buttonUpPending = false;
            }
        }
        #endregion

        #region En/Disable
        private void Disable()
        {
            MouseLeave -= MouseLeaveHandler;
            base.IsEnabled = false;

            //

            _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);

            _disable_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _disable_border_animation);

            _disable_border_special_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _disable_border_special_animation);

            _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _disable_background_animation);
        }

        private void Enable()
        {
            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

            _idle_border_special_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _idle_border_special_animation);

            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

            //

            base.IsEnabled = true;
            MouseLeave += MouseLeaveHandler;
        }
        #endregion

        //

        private static class Colors
        {
            internal static class DarkMode
            {
                internal static readonly SolidColorBrush FocusVisual = Brushes.White;

                internal static readonly SolidColorBrush IdleFont = Brushes.White;
                internal static readonly SolidColorBrush IdleBorder = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush IdleBorderSpecial = new(Color.FromRgb(0x35, 0x35, 0x35));
                internal static readonly SolidColorBrush IdleBackground = new(Color.FromRgb(0x2d, 0x2d, 0x2d));

                internal static readonly SolidColorBrush MouseOverBackground = new(Color.FromRgb(0x32, 0x32, 0x32));

                internal static readonly SolidColorBrush MouseDownFont = new(Color.FromRgb(0xce, 0xce, 0xce));
                internal static readonly SolidColorBrush MouseDownBorder = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush MouseDownBorderSpecial = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush MouseDownBackground = new(Color.FromRgb(0x27, 0x27, 0x27));

                internal static readonly SolidColorBrush DisabledFont = new(Color.FromRgb(0x78, 0x78, 0x78));
                internal static readonly SolidColorBrush DisabledBorder = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush DisabledBorderSpecial = new(Color.FromRgb(0x30, 0x30, 0x30));
                internal static readonly SolidColorBrush DisabledBackground = new(Color.FromRgb(0x2a, 0x2a, 0x2a));
            }

            internal static class LightMode
            {
                internal static readonly SolidColorBrush FocusVisual = new(Color.FromRgb(0x1a, 0x1a, 0x1a));

                internal static readonly SolidColorBrush IdleFont = new(Color.FromRgb(0x1b, 0x1b, 0x1b));
                internal static readonly SolidColorBrush IdleBorder = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
                internal static readonly SolidColorBrush IdleBorderSpecial = new(Color.FromRgb(0xcc, 0xcc, 0xcc));
                internal static readonly SolidColorBrush IdleBackground = new(Color.FromRgb(0xfb, 0xfb, 0xfb));

                internal static readonly SolidColorBrush MouseOverBackground = new(Color.FromRgb(0xf6, 0xf6, 0xf6));
                
                internal static readonly SolidColorBrush MouseDownFont = new(Color.FromRgb(0x5d, 0x5d, 0x5d));
                internal static readonly SolidColorBrush MouseDownBorder = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
                internal static readonly SolidColorBrush MouseDownBorderSpecial = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
                internal static readonly SolidColorBrush MouseDownBackground = new(Color.FromRgb(0xf5, 0xf5, 0xf5));

                internal static readonly SolidColorBrush DisabledFont = new(Color.FromRgb(0x9d, 0x9d, 0x9d));
                internal static readonly SolidColorBrush DisabledBorder = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
                internal static readonly SolidColorBrush DisabledBorderSpecial = new(Color.FromRgb(0xe5, 0xe5, 0xe5));
                internal static readonly SolidColorBrush DisabledBackground = new(Color.FromRgb(0xf5, 0xf5, 0xf5));
            }
        }

        private void ColorProviderChanged()
        {
            FrameworkElementFactory focusVisualFrameworkElementFactory = new(typeof(Border));
            focusVisualFrameworkElementFactory.SetValue(Border.BorderThicknessProperty, new Thickness(2d));
            focusVisualFrameworkElementFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(-3d));
            focusVisualFrameworkElementFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(7.5d));

            if (Theme.IsDarkMode)
            {
                focusVisualFrameworkElementFactory.SetValue(Border.BorderBrushProperty, Colors.DarkMode.FocusVisual);

                BorderThickness = new(0d, 1d, 0d, 0d);
                _buttonBackground.Margin = new(1d, 0d, 1d, 1d);

                _idle_font_animation.To = Colors.DarkMode.IdleFont;
                _idle_border_animation.To = Colors.DarkMode.IdleBorder;
                _idle_border_special_animation.To = Colors.DarkMode.IdleBorderSpecial;
                _idle_background_animation.To = Colors.DarkMode.IdleBackground;

                _mouse_over_background_animation.To = Colors.DarkMode.MouseOverBackground;

                _mouse_down_font_animation.To = Colors.DarkMode.MouseDownFont;
                _mouse_down_border_animation.To = Colors.DarkMode.MouseDownBorder;
                _mouse_down_border_special_animation.To = Colors.DarkMode.MouseDownBorderSpecial;
                _mouse_down_background_animation.To = Colors.DarkMode.MouseDownBackground;

                _disable_font_animation.To = Colors.DarkMode.DisabledFont;
                _disable_border_special_animation.To = Colors.DarkMode.DisabledBorderSpecial;
                _disable_border_animation.To = Colors.DarkMode.DisabledBorder;
                _disable_background_animation.To = Colors.DarkMode.DisabledBackground;
            }
            else
            {
                focusVisualFrameworkElementFactory.SetValue(Border.BorderBrushProperty, Colors.LightMode.FocusVisual);

                BorderThickness = new(0d, 0d, 0d, 1d);
                _buttonBackground.Margin = new(1d, 1d, 1d, 0d);

                _idle_font_animation.To = Colors.LightMode.IdleFont;
                _idle_border_animation.To = Colors.LightMode.IdleBorder;
                _idle_border_special_animation.To = Colors.LightMode.IdleBorderSpecial;
                _idle_background_animation.To = Colors.LightMode.IdleBackground;

                _mouse_over_background_animation.To = Colors.LightMode.MouseOverBackground;

                _mouse_down_font_animation.To = Colors.LightMode.MouseDownFont;
                _mouse_down_border_animation.To = Colors.LightMode.MouseDownBorder;
                _mouse_down_border_special_animation.To = Colors.LightMode.MouseDownBorderSpecial;
                _mouse_down_background_animation.To = Colors.LightMode.MouseDownBackground;

                _disable_font_animation.To = Colors.LightMode.DisabledFont;
                _disable_border_special_animation.To = Colors.LightMode.DisabledBorderSpecial;
                _disable_border_animation.To = Colors.LightMode.DisabledBorder;
                _disable_background_animation.To = Colors.LightMode.DisabledBackground;
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
                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

                _idle_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

                _idle_border_special_animation.From = (SolidColorBrush)BorderBrush;
                BeginAnimation(Border.BorderBrushProperty, _idle_border_special_animation);

                _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);
            }
            else
            {
                _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);

                _disable_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(Border.BackgroundProperty, _disable_border_animation);

                _disable_border_special_animation.From = (SolidColorBrush)BorderBrush;
                BeginAnimation(Border.BorderBrushProperty, _disable_border_special_animation);

                _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(Border.BackgroundProperty, _disable_background_animation);                
            }
        }
    }
}