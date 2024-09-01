using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal sealed class Button_Primary : Border
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

        new internal Boolean IsInitialized { get; private set; } = false;

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region Definitions
        private readonly Border _buttonBackground = new();
        private readonly TextBlock _textBlock = new(false);

        internal delegate void ClickHandler(Button_Primary sender);
        internal event ClickHandler Click;
        internal delegate void PreviewClickHandler();
        internal event PreviewClickHandler PreviewClick;
        #endregion

        public Button_Primary()
        {
            UseLayoutRounding = true;
            CornerRadius = new(5d);
            Focusable = true;
            Height = 32d;
            Width = 152d;
            BorderThickness = new(0d, 0d, 0d, 1d);
            Child = _buttonBackground;

            _buttonBackground.Margin = new(1d, 1d, 1d, 0d);
            _buttonBackground.CornerRadius = new(4d);
            _buttonBackground.Child = _textBlock;

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
                    Background = AccentColors.DarkMode.IdleBorderAsBrush;
                    BorderBrush = AccentColors.DarkMode.IdleBorderBottomAsBrush;
                    _buttonBackground.Background = AccentColors.DarkMode.IdleAsBrush;
                    _textBlock.Foreground = Brushes.Black;
                }
                else
                {
                    Background = AccentColors.LightMode.IdleBorderAsBrush;
                    BorderBrush = AccentColors.LightMode.IdleBorderBottomAsBrush;
                    _buttonBackground.Background = AccentColors.LightMode.IdleAsBrush;
                    _textBlock.Foreground = Brushes.White;
                }
            }
            else
            {
                if (Theme.IsDarkMode)
                {
                    Background = Colors.DarkMode.DisabledBorderBackground;
                    BorderBrush = Colors.DarkMode.DisabledBorderBackground;
                    _buttonBackground.Background = Colors.DarkMode.DisabledBorderBackground;
                    _textBlock.Foreground = Colors.DarkMode.DisabledFontColor;
                }
                else
                {
                    Background = Colors.LightMode.DisabledBorderBackground;
                    BorderBrush = Colors.LightMode.DisabledBorderBackground;
                    _buttonBackground.Background = Colors.LightMode.DisabledBorderBackground;
                    _textBlock.Foreground = Brushes.White;
                }
            }

            ColorProviderChanged();

            IsInitialized = true;
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        #region Animation Definitions
        private readonly SolidColorBrushAnimation _idle_font_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_border_bottom_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _idle_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_over_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_over_border_bottom_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_over_background_animation = new() { Duration = UI.LongAnimationDuration };

        private readonly SolidColorBrushAnimation _mouse_down_font_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_border_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_border_bottom_animation = new() { Duration = UI.ShortAnimationDuration };
        private readonly SolidColorBrushAnimation _mouse_down_background_animation = new() { Duration = UI.ShortAnimationDuration };

        private readonly SolidColorBrushAnimation _disable_font_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_border_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_border_bottom_animation = new() { Duration = UI.LongAnimationDuration };
        private readonly SolidColorBrushAnimation _disable_background_animation = new() { Duration = UI.LongAnimationDuration };

        private Boolean _buttonUpPending = false;

        private void BeginButtonDownAnimation()
        {
            _mouse_down_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _mouse_down_font_animation);

            _mouse_down_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _mouse_down_border_animation);

            _mouse_down_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _mouse_down_border_bottom_animation);

            _mouse_down_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _mouse_down_background_animation);

            _buttonUpPending = true;
        }

        private void BeginButtonUpAnimation()
        {
            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

            _mouse_over_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _mouse_over_border_animation);

            _mouse_over_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _mouse_over_border_bottom_animation);

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
        private void MouseEnterHandler(Object sender, MouseEventArgs e)
        {
            _mouse_over_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _mouse_over_border_animation);

            _mouse_over_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _mouse_over_border_bottom_animation);

            _mouse_over_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _mouse_over_background_animation);
        }

        private void PreviewMouseDownHandler(Object sender, MouseButtonEventArgs e) => BeginButtonDownAnimation();

        private void PreviewMouseUpHandler(Object sender, MouseButtonEventArgs e)
        {
            PreviewClick?.Invoke();

            BeginButtonUpAnimation();

            Click?.Invoke(this);
        }

        private void MouseLeaveHandler(Object sender, MouseEventArgs e)
        {
            if (_buttonUpPending) // when user presses mouse key down, then drags out of the object (skips mouse up event)
            {
                _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

                _buttonUpPending = false;
            }

            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

            _idle_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _idle_border_bottom_animation);

            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);
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

            _disable_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _disable_border_bottom_animation);

            _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _disable_background_animation);
        }

        private void Enable()
        {
            _idle_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
            _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _idle_font_animation);

            _idle_border_animation.From = (SolidColorBrush)Background;
            BeginAnimation(Border.BackgroundProperty, _idle_border_animation);

            _idle_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
            BeginAnimation(Border.BorderBrushProperty, _idle_border_bottom_animation);

            _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
            _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);

            //

            base.IsEnabled = true;
            MouseLeave += MouseLeaveHandler;
        }
        #endregion

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private static class Colors
        {
            internal static class DarkMode
            {
                internal static readonly SolidColorBrush MouseDownFontColor = new (Color.FromRgb(0x2d, 0x2d, 0x2d));

                internal static readonly SolidColorBrush DisabledBorderBackground = new(Color.FromRgb(0x43, 0x43, 0x43));

                internal static readonly SolidColorBrush DisabledFontColor = new(Color.FromRgb(0xa7, 0xa7, 0xa7));
            }

            internal static class LightMode
            {
                internal static readonly SolidColorBrush MouseDownFontColor = new(Color.FromRgb(0xeb, 0xeb, 0xeb));

                internal static readonly SolidColorBrush DisabledBorderBackground = new(Color.FromRgb(0xbf, 0xbf, 0xbf));
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
                focusVisualFrameworkElementFactory.SetValue(Border.BorderBrushProperty, AccentColors.DarkMode.FocusVisualAsBrush);

                _idle_font_animation.To = Brushes.Black;
                _idle_border_bottom_animation.To = AccentColors.DarkMode.IdleBorderBottomAsBrush;
                _idle_border_animation.To = AccentColors.DarkMode.IdleBorderAsBrush;
                _idle_background_animation.To = AccentColors.DarkMode.IdleAsBrush;

                _mouse_over_border_animation.To = AccentColors.DarkMode.MouseOverBorderAsBrush;
                _mouse_over_border_bottom_animation.To = AccentColors.DarkMode.MouseOverBorderBottomAsBrush;
                _mouse_over_background_animation.To = AccentColors.DarkMode.MouseOverBackgroundAsBrush;

                _mouse_down_font_animation.To = Colors.DarkMode.MouseDownFontColor;
                _mouse_down_border_animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _mouse_down_border_bottom_animation.To = AccentColors.DarkMode.MouseDownAsBrush;
                _mouse_down_background_animation.To = AccentColors.DarkMode.MouseDownAsBrush;

                _disable_font_animation.To = Colors.DarkMode.DisabledFontColor;
                _disable_border_animation.To = Colors.DarkMode.DisabledBorderBackground;
                _disable_border_bottom_animation.To = Colors.DarkMode.DisabledBorderBackground;
                _disable_background_animation.To = Colors.DarkMode.DisabledBorderBackground;
            }
            else
            {
                focusVisualFrameworkElementFactory.SetValue(Border.BorderBrushProperty, AccentColors.LightMode.FocusVisualAsBrush);

                _idle_font_animation.To = Brushes.White;
                _idle_border_animation.To = AccentColors.LightMode.IdleBorderAsBrush;
                _idle_border_bottom_animation.To = AccentColors.LightMode.IdleBorderBottomAsBrush;
                _idle_background_animation.To = AccentColors.LightMode.IdleAsBrush;

                _mouse_over_border_animation.To = AccentColors.LightMode.MouseOverBorderAsBrush;
                _mouse_over_border_bottom_animation.To = AccentColors.LightMode.MouseOverBorderBottomAsBrush;
                _mouse_over_background_animation.To = AccentColors.LightMode.MouseOverBackgroundAsBrush;

                _mouse_down_font_animation.To = Colors.LightMode.MouseDownFontColor;
                _mouse_down_border_animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _mouse_down_border_bottom_animation.To = AccentColors.LightMode.MouseDownAsBrush;
                _mouse_down_background_animation.To = AccentColors.LightMode.MouseDownAsBrush;

                _disable_font_animation.To = Brushes.White;
                _disable_border_animation.To = Colors.LightMode.DisabledBorderBackground;
                _disable_border_bottom_animation.To = Colors.LightMode.DisabledBorderBackground;
                _disable_background_animation.To = Colors.LightMode.DisabledBorderBackground;
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

                _idle_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
                BeginAnimation(Border.BorderBrushProperty, _idle_border_bottom_animation);

                _idle_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(Border.BackgroundProperty, _idle_background_animation);
            }
            else
            {
                _disable_font_animation.From = (SolidColorBrush)_textBlock.Foreground;
                _textBlock.BeginAnimation(TextBlock.ForegroundProperty, _disable_font_animation);

                _disable_border_animation.From = (SolidColorBrush)Background;
                BeginAnimation(Border.BackgroundProperty, _disable_border_animation);

                _disable_border_bottom_animation.From = (SolidColorBrush)BorderBrush;
                BeginAnimation(Border.BorderBrushProperty, _disable_border_bottom_animation);

                _disable_background_animation.From = (SolidColorBrush)_buttonBackground.Background;
                _buttonBackground.BeginAnimation(Border.BackgroundProperty, _disable_background_animation);
            }
        }
    }
}